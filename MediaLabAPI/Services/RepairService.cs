using Microsoft.EntityFrameworkCore;
using MediaLabAPI.DTOs;
using MediaLabAPI.Models;
using MediaLabAPI.Data;

namespace MediaLabAPI.Services
{
    public class RepairService : IRepairService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RepairService> _logger;

        public RepairService(AppDbContext context, ILogger<RepairService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateRepairResponseDto> CreateRepairAsync(CreateRepairRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Gestisci Cliente
                Guid customerId;
                if (request.CustomerId.HasValue)
                {
                    customerId = request.CustomerId.Value;
                    var existingCustomer = await _context.C_ANA_Companies
                        .FirstOrDefaultAsync(c => c.Id == customerId && c.IsDeleted != true);

                    if (existingCustomer == null)
                        throw new ArgumentException("Cliente non trovato");
                }
                else if (request.NewCustomer != null)
                {
                    customerId = await CreateNewCustomerAsync(request.NewCustomer, request.MultitenantId);
                }
                else
                {
                    throw new ArgumentException("Specificare un cliente esistente o i dati per un nuovo cliente");
                }

                // 2. Gestisci Dispositivo
                Guid deviceGuid;

                if (request.DeviceId.HasValue)
                {
                    var deviceRegistryId = request.DeviceId.Value;
                    var existingDevice = await _context.DeviceRegistry
                        .FirstOrDefaultAsync(d => d.Id == deviceRegistryId && !d.IsDeleted);

                    if (existingDevice == null)
                        throw new ArgumentException("Dispositivo non trovato");

                    deviceGuid = existingDevice.DeviceId;
                }
                else if (request.NewDevice != null)
                {
                    var newDeviceResult = await CreateNewDeviceAsync(request.NewDevice, customerId, request.MultitenantId);
                    deviceGuid = newDeviceResult.DeviceId;
                }
                else
                {
                    throw new ArgumentException("Specificare un dispositivo esistente o i dati per un nuovo dispositivo");
                }

                // 3. Genera RepairCode univoco
                var repairCode = GenerateRepairCode();

                // 4. Crea Riparazione
                var repair = new DeviceRepair
                {
                    RepairId = Guid.NewGuid(),        // 🆕 GUID univoco
                    RepairCode = repairCode,          // 🆕 Codice ricercabile
                    DeviceId = deviceGuid,
                    CustomerId = customerId,
                    CompanyId = request.MultitenantId,
                    MultitenantId = request.MultitenantId,
                    FaultDeclared = request.RepairData.FaultDeclared,
                    RepairAction = request.RepairData.RepairAction,
                    TechnicianCode = request.RepairData.TechnicianCode,
                    TechnicianName = request.RepairData.TechnicianName,
                    RepairStatusCode = "RECEIVED",
                    RepairStatus = "Ricevuto",
                    CreatedAt = DateTime.Now,
                    ReceivedAt = DateTime.Now,
                    Notes = BuildNotesFromRequest(request)
                };

                _context.DeviceRepairs.Add(repair);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Repair created: ID={RepairId}, Code={RepairCode}", repair.Id, repair.RepairCode);

                // 5. 🆕 SALVA DIAGNOSTICA - LOGICA COMPLETA
                bool hasIncomingTest = false;
                if (request.DiagnosticItems?.Any() == true)
                {
                    _logger.LogInformation("🔍 Saving diagnostic data for repair {RepairCode}...", repair.RepairCode);
                    hasIncomingTest = await SaveDiagnosticDataAsync(repair, request.DiagnosticItems);
                    _logger.LogInformation("✅ Diagnostic data saved: {Success}", hasIncomingTest);
                }

                await transaction.CommitAsync();

                return new CreateRepairResponseDto
                {
                    RepairId = repair.Id,
                    RepairGuid = repair.RepairId,      // 🆕 GUID
                    RepairCode = repair.RepairCode,    // 🆕 Codice
                    CustomerId = customerId,
                    DeviceId = request.DeviceId,
                    CreatedAt = repair.CreatedAt,
                    Status = repair.RepairStatus,
                    Message = "Scheda di riparazione creata con successo",
                    HasIncomingTest = hasIncomingTest  // 🆕 Flag diagnostica
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error creating repair");
                throw;
            }
        }

        // 🆕 METODO COMPLETO PER SALVARE DIAGNOSTICA
        private async Task<bool> SaveDiagnosticDataAsync(DeviceRepair repair, List<DiagnosticItemDto> diagnosticItems)
        {
            try
            {
                if (diagnosticItems == null || !diagnosticItems.Any())
                {
                    _logger.LogInformation("No diagnostic items to save");
                    return false;
                }

                // Crea l'entità IncomingTest
                var incomingTest = new IncomingTest
                {
                    RepairId = repair.RepairId,           // 🔧 Collega alla riparazione tramite GUID
                    CompanyId = repair.CompanyId,
                    MultitenantId = repair.MultitenantId,
                    CreatedData = DateTime.Now,
                    IsDeleted = false
                };

                // 🔧 MAPPING DIAGNOSTICA FRONTEND → DATABASE
                foreach (var item in diagnosticItems.Where(d => d.Active))
                {
                    switch (item.Id.ToLower())
                    {
                        case "battery":
                            incomingTest.Batteria = true;
                            break;
                        case "wifi":
                            incomingTest.WiFi = true;
                            break;
                        case "cellular":
                        case "rf-cellular":
                            incomingTest.Rete = true;
                            break;
                        case "camera":
                            incomingTest.FotocameraPosteriore = true;
                            incomingTest.FotocameraAnteriore = true;
                            break;
                        case "face-id":
                            incomingTest.FaceId = true;
                            break;
                        case "scanner":
                        case "touch-id":
                            incomingTest.TouchId = true;
                            break;
                        case "sensors":
                            incomingTest.SensoreDiProssimita = true;
                            break;
                        case "system":
                            incomingTest.SchedaMadre = true;
                            break;
                        case "bluetooth":
                            // Non c'è un campo specifico, aggiungi a note o ignora
                            break;
                        case "device-info":
                            // Info generali, non mappato direttamente
                            break;
                        case "apple-pay":
                            // Non mappato direttamente nel DB attuale
                            break;
                        case "clock":
                        case "services":
                        case "software":
                            // Considerati come test di sistema
                            incomingTest.SchedaMadre = true;
                            break;
                        case "sim":
                            // Potrebbe essere mappato su rete
                            incomingTest.Rete = true;
                            break;
                        case "magsafe":
                            // Non mappato direttamente
                            break;
                        case "wireless-problem":
                            // Indica problemi wireless
                            incomingTest.WiFi = false; // Test fallito
                            break;
                        default:
                            _logger.LogWarning("⚠️ Unknown diagnostic item: {ItemId}", item.Id);
                            break;
                    }
                }

                // Salva nel database
                _context.IncomingTests.Add(incomingTest);
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ IncomingTest saved with ID: {TestId} for Repair: {RepairCode}",
                    incomingTest.Id, repair.RepairCode);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error saving diagnostic data for repair {RepairCode}", repair.RepairCode);
                throw; // Rilancia per far fallire la transazione
            }
        }

        // 🆕 METODO AGGIORNATO PER GENERARE CODICE UNIVOCO
        private string GenerateRepairCode()
        {
            var now = DateTime.Now;
            var dateComponent = now.ToString("yyyyMMdd");
            var timeComponent = now.ToString("HHmmss");
            var randomComponent = new Random().Next(100, 999);

            return $"REP{dateComponent}{timeComponent}{randomComponent}";
        }

        // 🆕 METODO PER CERCARE PER CODICE
        public async Task<DeviceRepair?> GetRepairByCodeAsync(string repairCode)
        {
            if (string.IsNullOrWhiteSpace(repairCode))
                return null;

            return await _context.DeviceRepairs
                .Include(r => r.Device)
                .Include(r => r.Customer)
                .Include(r => r.Company)
                .Include(r => r.IncomingTest)  // 🆕 Include diagnostica
                .FirstOrDefaultAsync(r => r.RepairCode == repairCode && !r.IsDeleted);
        }

        // METODI ESISTENTI AGGIORNATI CON INCLUDE DIAGNOSTICA

        public async Task<DeviceRepair?> GetRepairByIdAsync(int id)
        {
            return await _context.DeviceRepairs
                .Include(r => r.Device)
                .Include(r => r.Customer)
                .Include(r => r.Company)
                .Include(r => r.IncomingTest)  // 🆕 Include diagnostica
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<DeviceRepair>> GetRepairsAsync(Guid? multitenantId, string? status)
        {
            var query = _context.DeviceRepairs
                .Include(r => r.Device)
                .Include(r => r.Customer)
                .Include(r => r.Company)
                .Include(r => r.IncomingTest)  // 🆕 Include diagnostica
                .Where(r => !r.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(r => r.MultitenantId == multitenantId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.RepairStatusCode == status);

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceRepair>> GetRepairsByCustomerAsync(Guid customerId)
        {
            return await _context.DeviceRepairs
                .Include(r => r.Device)
                .Include(r => r.Company)
                .Include(r => r.IncomingTest)  // 🆕 Include diagnostica
                .Where(r => r.CustomerId == customerId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceRepair>> GetRepairsByDeviceAsync(int deviceRegistryId)
        {
            var device = await _context.DeviceRegistry
                .FirstOrDefaultAsync(d => d.Id == deviceRegistryId && !d.IsDeleted);

            if (device == null)
                return new List<DeviceRepair>();

            return await _context.DeviceRepairs
                .Include(r => r.Customer)
                .Include(r => r.Company)
                .Include(r => r.IncomingTest)  // 🆕 Include diagnostica
                .Where(r => r.DeviceId == device.DeviceId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateRepairStatusAsync(int repairId, string statusCode, string status, string? notes)
        {
            var repair = await _context.DeviceRepairs
                .FirstOrDefaultAsync(r => r.Id == repairId && !r.IsDeleted);

            if (repair == null)
                throw new ArgumentException("Riparazione non trovata");

            repair.RepairStatusCode = statusCode;
            repair.RepairStatus = status;
            repair.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrEmpty(notes))
            {
                repair.Notes = string.IsNullOrEmpty(repair.Notes)
                    ? notes
                    : $"{repair.Notes} | {notes}";
            }

            // Aggiorna timestamp specifici
            switch (statusCode.ToUpper())
            {
                case "STARTED":
                    repair.StartedAt = DateTime.Now;
                    break;
                case "COMPLETED":
                    repair.CompletedAt = DateTime.Now;
                    break;
                case "DELIVERED":
                    repair.DeliveredAt = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
        }

        // METODI HELPER ESISTENTI (mantengo invariati)

        private async Task<Guid> CreateNewCustomerAsync(CustomerDataDto customerData, Guid multitenantId)
        {
            var customer = new C_ANA_Company
            {
                Id = Guid.NewGuid(),
                Tipologia = customerData.Tipo == "Privato" ? "1" : "0",
                RagioneSociale = customerData.Tipo == "Privato"
                    ? $"{customerData.Cognome} {customerData.Nome}".Trim()
                    : customerData.RagioneSociale ?? "",
                Cognome = customerData.Tipo == "Privato" ? customerData.Cognome : null,
                Nome = customerData.Tipo == "Privato" ? customerData.Nome : null,
                Indirizzo = customerData.Indirizzo,
                Cap = customerData.Cap,
                Regione = customerData.Regione,
                Provincia = customerData.Provincia,
                Citta = customerData.Citta,
                Telefono = customerData.Telefono,
                EmailAziendale = customerData.Email,
                FiscalCode = customerData.CodiceFiscale,
                PIva = customerData.PartitaIva,
                EmailPec = customerData.EmailPec,
                CodiceSdi = customerData.CodiceSdi,
                IBAN = customerData.Iban,
                isCustomer = customerData.Cliente,
                isSupplier = customerData.Fornitore,
                MultiTenantId = multitenantId,
                CreatedAt = DateTime.Now,
                active = true,
                IsDeleted = false,
                EnabledFE = false,
                IsVendolo = false,
                IsVendoloFE = false,
                isTenant = false
            };

            _context.C_ANA_Companies.Add(customer);
            await _context.SaveChangesAsync();

            return customer.Id;
        }

        private async Task<(int Id, Guid DeviceId)> CreateNewDeviceAsync(DeviceDataDto deviceData, Guid customerId, Guid multitenantId)
        {
            var device = new DeviceRegistry
            {
                DeviceId = Guid.NewGuid(),
                CustomerId = customerId,
                CompanyId = multitenantId,
                MultitenantId = multitenantId,
                SerialNumber = deviceData.SerialNumber,
                Brand = deviceData.Brand,
                Model = deviceData.Model,
                DeviceType = deviceData.DeviceType,
                PurchaseDate = deviceData.PurchaseDate.HasValue
                    ? DateOnly.FromDateTime(deviceData.PurchaseDate.Value)
                    : null,
                ReceiptNumber = deviceData.ReceiptNumber,
                Retailer = deviceData.Retailer,
                Notes = deviceData.Notes,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _context.DeviceRegistry.Add(device);
            await _context.SaveChangesAsync();

            return (device.Id, device.DeviceId);
        }

        private string BuildNotesFromRequest(CreateRepairRequestDto request)
        {
            var notes = new List<string>();

            if (!string.IsNullOrEmpty(request.Notes))
                notes.Add($"Note: {request.Notes}");

            if (!string.IsNullOrEmpty(request.RepairData.UnlockCode))
                notes.Add($"Codice Sblocco: {request.RepairData.UnlockCode}");

            if (!string.IsNullOrEmpty(request.RepairData.CourtesyPhone))
                notes.Add($"Telefono Cortesia: {request.RepairData.CourtesyPhone}");

            if (request.NewDevice?.Color != null)
                notes.Add($"Colore: {request.NewDevice.Color}");

            // Aggiungi diagnostica attiva
            var activeDiagnostics = request.DiagnosticItems?
                .Where(d => d.Active)
                .Select(d => d.Label) ?? Enumerable.Empty<string>();

            if (activeDiagnostics.Any())
                notes.Add($"Diagnostica OK: {string.Join(", ", activeDiagnostics)}");

            // Aggiungi info prezzo se presente
            if (request.RepairData.EstimatedPrice.HasValue)
                notes.Add($"Preventivo: €{request.RepairData.EstimatedPrice:F2}");

            if (!string.IsNullOrEmpty(request.RepairData.PaymentType))
                notes.Add($"Pagamento: {request.RepairData.PaymentType}");

            return string.Join(" | ", notes);
        }
    }
}