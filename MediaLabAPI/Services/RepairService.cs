using Microsoft.EntityFrameworkCore;
using MediaLabAPI.DTOs;
using MediaLabAPI.Models;
using MediaLabAPI.Data;
using Microsoft.Extensions.Logging;

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

                    _logger.LogInformation("🔍 Using existing customer: {CustomerId}", customerId);
                }
                else if (request.NewCustomer != null)
                {
                    _logger.LogInformation("🆕 Creating new customer: {CustomerName}",
                        $"{request.NewCustomer.Cognome} {request.NewCustomer.Nome}".Trim());
                    customerId = await CreateNewCustomerAsync(request.NewCustomer, request.MultitenantId);
                    _logger.LogInformation("✅ New customer created: {CustomerId}", customerId);
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
                    _logger.LogInformation("🔍 Using existing device: {DeviceId} - {Brand} {Model}",
                        existingDevice.DeviceId, existingDevice.Brand, existingDevice.Model);
                }
                else if (request.NewDevice != null)
                {
                    _logger.LogInformation("🆕 Creating new device: {Brand} {Model} - {SerialNumber}",
                        request.NewDevice.Brand, request.NewDevice.Model, request.NewDevice.SerialNumber);
                    var newDeviceResult = await CreateNewDeviceAsync(request.NewDevice, customerId, request.MultitenantId);
                    deviceGuid = newDeviceResult.DeviceId;
                    _logger.LogInformation("✅ New device created: {DeviceId}", deviceGuid);
                }
                else
                {
                    throw new ArgumentException("Specificare un dispositivo esistente o i dati per un nuovo dispositivo");
                }

                // 3. Genera RepairCode univoco
                var repairCode = GenerateRepairCode();
                _logger.LogInformation("🔢 Generated repair code: {RepairCode}", repairCode);

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

                _logger.LogInformation("✅ Repair created: ID={RepairId}, GUID={RepairGuid}, Code={RepairCode}",
                    repair.Id, repair.RepairId, repair.RepairCode);

                // 5. 🆕 SALVA DIAGNOSTICA - LOGICA COMPLETA CON SUMMARY
                bool hasIncomingTest = false;
                string diagnosticSummary = null;

                if (request.DiagnosticItems?.Any() == true)
                {
                    _logger.LogInformation("🔍 Processing {DiagnosticCount} diagnostic items for repair {RepairCode}...",
                        request.DiagnosticItems.Count, repair.RepairCode);

                    // Genera il summary prima di salvare
                    diagnosticSummary = GenerateDiagnosticSummary(request.DiagnosticItems);
                    _logger.LogInformation("📋 Diagnostic summary: {Summary}", diagnosticSummary);

                    // Salva la diagnostica nel database
                    hasIncomingTest = await SaveDiagnosticDataAsync(repair, request.DiagnosticItems);

                    if (hasIncomingTest)
                    {
                        _logger.LogInformation("✅ Diagnostic data saved successfully for repair {RepairCode}", repair.RepairCode);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ No diagnostic data was saved for repair {RepairCode}", repair.RepairCode);
                    }
                }
                else
                {
                    _logger.LogInformation("ℹ️ No diagnostic items provided for repair {RepairCode}", repair.RepairCode);
                    diagnosticSummary = "Nessun test diagnostico eseguito";
                }

                // 6. 🎯 FINALIZZA TRANSAZIONE
                await transaction.CommitAsync();
                _logger.LogInformation("🎉 Repair creation completed successfully for {RepairCode}", repair.RepairCode);

                // 7. 📤 PREPARA RESPONSE COMPLETA
                var response = new CreateRepairResponseDto
                {
                    RepairId = repair.Id,
                    RepairGuid = repair.RepairId,           // 🆕 GUID per update
                    RepairCode = repair.RepairCode,         // 🆕 Codice ricercabile
                    CustomerId = customerId,
                    DeviceId = request.DeviceId,
                    CreatedAt = repair.CreatedAt,
                    Status = repair.RepairStatus,
                    Message = $"Scheda di riparazione creata con successo. Codice: {repair.RepairCode}",
                    HasIncomingTest = hasIncomingTest,      // 🆕 Flag diagnostica
                    IncomingTestSummary = diagnosticSummary  // 🆕 Summary diagnostica
                };

                _logger.LogInformation("📋 Response prepared: RepairId={RepairId}, RepairGuid={RepairGuid}, HasDiagnostic={HasDiagnostic}",
                    response.RepairId, response.RepairGuid, response.HasIncomingTest);

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error creating repair for customer {CustomerId}. Request: {@Request}",
                    request.CustomerId, new
                    {
                        Customer = request.CustomerId,
                        Device = request.DeviceId,
                        FaultDeclared = request.RepairData?.FaultDeclared,
                        DiagnosticItemsCount = request.DiagnosticItems?.Count ?? 0
                    });
                throw;
            }
        }

        // 🔧 METODO PULITO PER SALVARE DIAGNOSTICA - SOLO MAPPING DIRETTO
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

                // 🎯 MAPPING DIRETTO DIAGNOSTICA FRONTEND → DATABASE
                // Mappiamo SOLO gli elementi attivamente selezionati dal frontend
                foreach (var item in diagnosticItems.Where(d => d.Active))
                {
                    switch (item.Id.ToLower())
                    {
                        case "device-info":
                            // Info dispositivo verificate - telefono funzionante
                            incomingTest.TelefonoSpento = false;
                            break;

                        case "battery":
                            incomingTest.Batteria = true;
                            break;

                        case "camera":
                            incomingTest.FotocameraPosteriore = true;
                            incomingTest.FotocameraAnteriore = true;
                            break;

                        case "cellular":
                            incomingTest.Rete = true;
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
                        case "clock":
                        case "services":
                        case "software":
                            incomingTest.SchedaMadre = true;
                            break;

                        case "wifi":
                            incomingTest.WiFi = true;
                            break;

                        case "rf-cellular":
                            incomingTest.Chiamata = true;
                            break;

                        case "sim":
                            // SIM funzionante = rete attiva
                            incomingTest.Rete = true;
                            break;

                        // 🔧 ELEMENTI HARDWARE FISICI (se presenti nel frontend)
                        case "touchscreen":
                            incomingTest.Touchscreen = true;
                            break;

                        case "lcd":
                        case "display":
                            incomingTest.Lcd = true;
                            break;

                        case "dock-ricarica":
                        case "charging-port":
                            incomingTest.DockDiRicarica = true;
                            break;

                        case "speaker":
                        case "audio":
                            incomingTest.SpeakerBuzzer = true;
                            break;

                        case "microphone":
                        case "microfono":
                            incomingTest.MicrofonoChiamate = true;
                            break;

                        case "volume-buttons":
                        case "tasti-volume":
                            incomingTest.TastiVolumeMuto = true;
                            break;

                        case "power-button":
                        case "tasto-power":
                            incomingTest.TastoStandbyPower = true;
                            break;

                        case "home-button":
                        case "tasto-home":
                            incomingTest.TastoHome = true;
                            break;

                        case "back-cover":
                        case "cover-posteriore":
                            incomingTest.BackCover = true;
                            break;

                        case "frame":
                        case "telaio":
                            incomingTest.Telaio = true;
                            break;

                        // ⚠️ ELEMENTI CON PROBLEMI
                        case "wireless-problem":
                            // Problema wireless rilevato
                            incomingTest.WiFi = false;
                            break;

                        // 📝 ELEMENTI NON MAPPATI - Solo log informativo
                        case "apple-pay":
                        case "bluetooth":
                        case "magsafe":
                            _logger.LogInformation("Diagnostic item '{ItemId}' ({Label}) passed but not mapped to specific DB field",
                                item.Id, item.Label);
                            break;

                        // 🎯 ELEMENTI SCONOSCIUTI
                        default:
                            _logger.LogWarning("⚠️ Unknown diagnostic item '{ItemId}' with label '{Label}' - no mapping available",
                                item.Id, item.Label);
                            break;
                    }
                }

                // Salva nel database
                _context.IncomingTests.Add(incomingTest);
                await _context.SaveChangesAsync();

                // 📊 LOG DETTAGLIATO
                var mappedItems = diagnosticItems.Where(d => d.Active).Select(d => d.Label).ToList();
                var mappedCount = diagnosticItems.Count(d => d.Active);

                _logger.LogInformation("✅ IncomingTest saved with ID: {TestId} for Repair: {RepairCode}. " +
                                      "Mapped {MappedCount} active diagnostic items: {MappedItems}",
                    incomingTest.Id, repair.RepairCode, mappedCount, string.Join(", ", mappedItems));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error saving diagnostic data for repair {RepairCode}", repair.RepairCode);
                throw; // Rilancia per far fallire la transazione
            }
        }


        // 🆕 METODO HELPER PER GENERARE SUMMARY DIAGNOSTICA
        private string GenerateDiagnosticSummary(List<DiagnosticItemDto> diagnosticItems)
        {
            if (diagnosticItems == null || !diagnosticItems.Any())
                return "Nessun test diagnostico eseguito";

            var activeTests = diagnosticItems.Where(d => d.Active).ToList();
            var inactiveTests = diagnosticItems.Where(d => !d.Active).ToList();

            if (!activeTests.Any() && !inactiveTests.Any())
                return "Nessun test diagnostico valido";

            var summaryParts = new List<string>();

            if (activeTests.Any())
            {
                var activeLabels = activeTests.Select(t => t.Label).ToList();
                summaryParts.Add($"✅ Funzionanti ({activeTests.Count}): {string.Join(", ", activeLabels)}");
            }

            if (inactiveTests.Any())
            {
                var inactiveLabels = inactiveTests.Select(t => t.Label).ToList();
                summaryParts.Add($"❌ Problemi rilevati ({inactiveTests.Count}): {string.Join(", ", inactiveLabels)}");
            }

            return string.Join(" | ", summaryParts);
        }

        // 🔧 METODO AGGIORNATO PER GENERARE CODICE (già esistente ma migliorato)
        private string GenerateRepairCode()
        {
            var now = DateTime.Now;
            var dateComponent = now.ToString("yyyyMMdd");
            var timeComponent = now.ToString("HHmmss");
            var randomComponent = new Random().Next(100, 999);

            var code = $"REP{dateComponent}{timeComponent}{randomComponent}";

            _logger.LogDebug("🔢 Generated repair code: {Code} at {Timestamp}", code, now);

            return code;
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
                Email = customerData.Email,
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

        // METODO PER AGIORNARE TUTTI I DATI ESISTENTI
        // 🆕 VERSIONE MIGLIORATA CON VALIDAZIONI E OTTIMIZZAZIONI
        public async Task UpdateRepairAsync(Guid repairId, UpdateRepairRequestDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1️⃣ VALIDAZIONI PRELIMINARI
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                var repair = await _context.DeviceRepairs
                    .Include(r => r.IncomingTest) // 🔧 Include per gestione diagnostica
                    .FirstOrDefaultAsync(r => r.RepairId == repairId && !r.IsDeleted);

                if (repair == null)
                    throw new ArgumentException("Riparazione non trovata");

                // 🚫 BLOCCA MODIFICHE SU RIPARAZIONI COMPLETATE/CONSEGNATE
                if (repair.RepairStatusCode.ToUpper() is "COMPLETED" or "DELIVERED")
                {
                    _logger.LogWarning("⚠️ Tentativo di modifica riparazione completata: {RepairCode}", repair.RepairCode);
                    throw new InvalidOperationException("Impossibile modificare una riparazione completata o consegnata");
                }

                var changes = new List<string>(); // 📝 Track delle modifiche per audit

                // 2️⃣ AGGIORNAMENTO CLIENTE (con validazione)
                if (request.CustomerId.HasValue && request.CustomerId.Value != repair.CustomerId)
                {
                    var customerExists = await _context.C_ANA_Companies
                        .AnyAsync(c => c.Id == request.CustomerId.Value && c.IsDeleted != true);

                    if (!customerExists)
                        throw new ArgumentException("Cliente specificato non trovato");

                    var oldCustomerId = repair.CustomerId;
                    repair.CustomerId = request.CustomerId.Value;
                    changes.Add($"Cliente: {oldCustomerId} → {request.CustomerId}");
                }

                // 3️⃣ AGGIORNAMENTO DISPOSITIVO (con validazione)
                if (request.DeviceId.HasValue)
                {
                    var deviceRegistry = await _context.DeviceRegistry
                        .FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId.Value && !d.IsDeleted);

                    if (deviceRegistry == null)
                        throw new ArgumentException("Dispositivo non trovato");

                    if (deviceRegistry.DeviceId != repair.DeviceId)
                    {
                        var oldDeviceId = repair.DeviceId;
                        repair.DeviceId = deviceRegistry.DeviceId;
                        changes.Add($"Dispositivo: {oldDeviceId} → {deviceRegistry.DeviceId}");
                    }
                }

                // 4️⃣ AGGIORNAMENTO DATI RIPARAZIONE (con merge intelligente)
                if (request.RepairData != null)
                {
                    // 🔄 Aggiorna solo i campi non vuoti (partial update)
                    if (!string.IsNullOrWhiteSpace(request.RepairData.FaultDeclared))
                    {
                        if (repair.FaultDeclared != request.RepairData.FaultDeclared)
                        {
                            changes.Add($"Guasto: '{repair.FaultDeclared}' → '{request.RepairData.FaultDeclared}'");
                            repair.FaultDeclared = request.RepairData.FaultDeclared;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(request.RepairData.RepairAction))
                    {
                        if (repair.RepairAction != request.RepairData.RepairAction)
                        {
                            changes.Add($"Azione: '{repair.RepairAction}' → '{request.RepairData.RepairAction}'");
                            repair.RepairAction = request.RepairData.RepairAction;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(request.RepairData.TechnicianCode))
                    {
                        if (repair.TechnicianCode != request.RepairData.TechnicianCode)
                        {
                            changes.Add($"Tecnico: '{repair.TechnicianCode}' → '{request.RepairData.TechnicianCode}'");
                            repair.TechnicianCode = request.RepairData.TechnicianCode;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(request.RepairData.TechnicianName))
                    {
                        if (repair.TechnicianName != request.RepairData.TechnicianName)
                        {
                            changes.Add($"Nome Tecnico: '{repair.TechnicianName}' → '{request.RepairData.TechnicianName}'");
                            repair.TechnicianName = request.RepairData.TechnicianName;
                        }
                    }
                }

                // 5️⃣ GESTIONE INTELLIGENTE NOTE (append vs replace)
                if (!string.IsNullOrWhiteSpace(request.Notes))
                {
                    var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    var newNote = $"[{timestamp}] {request.Notes}";

                    repair.Notes = string.IsNullOrWhiteSpace(repair.Notes)
                        ? newNote
                        : $"{repair.Notes} | {newNote}";

                    changes.Add($"Nota aggiunta: {request.Notes}");
                }

                // 6️⃣ GESTIONE DIAGNOSTICA AVANZATA (con confronto)
                if (request.DiagnosticItems != null)
                {
                    var hasActiveDiagnostics = request.DiagnosticItems.Any(d => d.Active);

                    if (hasActiveDiagnostics)
                    {
                        // 🗑️ Rimuovi diagnostica esistente
                        if (repair.IncomingTest != null)
                        {
                            _context.IncomingTests.Remove(repair.IncomingTest);
                            changes.Add("Diagnostica precedente rimossa");
                        }

                        // 💾 Salva nuova diagnostica
                        var diagnosticSaved = await SaveDiagnosticDataAsync(repair, request.DiagnosticItems);
                        if (diagnosticSaved)
                        {
                            var activeDiagnosticLabels = request.DiagnosticItems
                                .Where(d => d.Active)
                                .Select(d => d.Label)
                                .ToList();
                            changes.Add($"Diagnostica aggiornata: {string.Join(", ", activeDiagnosticLabels)}");
                        }
                    }
                    else if (repair.IncomingTest != null)
                    {
                        // 🗑️ Rimuovi diagnostica se nessun test è attivo
                        _context.IncomingTests.Remove(repair.IncomingTest);
                        changes.Add("Diagnostica rimossa (nessun test attivo)");
                    }
                }

                // 7️⃣ AGGIORNAMENTO TIMESTAMP E AUDIT
                repair.UpdatedAt = DateTime.Now;

                // 📝 LOG DETTAGLIATO DELLE MODIFICHE
                if (changes.Any())
                {
                    var changeLog = string.Join(" | ", changes);
                    _logger.LogInformation("🔄 Repair {RepairCode} updated: {Changes}", repair.RepairCode, changeLog);

                    // 💡 OPZIONALE: Salva audit trail in tabella separata
                    // await SaveAuditTrailAsync(repair.Id, "UPDATE", changeLog);
                }
                else
                {
                    _logger.LogInformation("ℹ️ No changes detected for repair {RepairCode}", repair.RepairCode);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("✅ Repair {RepairCode} successfully updated with {ChangeCount} changes",
                    repair.RepairCode, changes.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Error updating repair {RepairId}", repairId);
                throw;
            }
        }

        // METODO PER RECUPERARE I DATI DELLE RIPARAZIONI
        public async Task<IEnumerable<RepairDetailDto>> GetRepairsLightAsync(RepairSearchRequestDto searchRequest)
        {
            try
            {
                _logger.LogInformation("🔍 Starting light repairs search with filters: {@Filters}",
                    new
                    {
                        searchRequest.SearchQuery,
                        searchRequest.RepairCode,
                        searchRequest.StatusCode,
                        searchRequest.Page,
                        searchRequest.PageSize
                    });

                var query = _context.DeviceRepairs
                    .Include(r => r.Device)
                    .Include(r => r.Customer)
                    .Where(r => !r.IsDeleted);

                if (!string.IsNullOrWhiteSpace(searchRequest.SearchQuery))
                {
                    var searchTerm = searchRequest.SearchQuery.ToLower();

                    query = query.Where(r =>
                        r.RepairCode.ToLower().Contains(searchTerm) ||
                        (r.Customer != null && (
                            (r.Customer.RagioneSociale != null && r.Customer.RagioneSociale.ToLower().Contains(searchTerm)) ||
                            (r.Customer.Cognome != null && r.Customer.Cognome.ToLower().Contains(searchTerm)) ||
                            (r.Customer.Nome != null && r.Customer.Nome.ToLower().Contains(searchTerm))
                        )) ||
                        (r.Device != null && (
                            (r.Device.Brand != null && r.Device.Brand.ToLower().Contains(searchTerm)) ||
                            (r.Device.Model != null && r.Device.Model.ToLower().Contains(searchTerm)) ||
                            (r.Device.SerialNumber != null && r.Device.SerialNumber.ToLower().Contains(searchTerm))
                        ))
                    );
                }

                if (!string.IsNullOrWhiteSpace(searchRequest.RepairCode))
                    query = query.Where(r => r.RepairCode == searchRequest.RepairCode);

                if (searchRequest.CustomerId.HasValue)
                    query = query.Where(r => r.CustomerId == searchRequest.CustomerId.Value);

                if (!string.IsNullOrWhiteSpace(searchRequest.StatusCode))
                    query = query.Where(r => r.RepairStatusCode == searchRequest.StatusCode);

                if (searchRequest.FromDate.HasValue)
                    query = query.Where(r => r.CreatedAt >= searchRequest.FromDate.Value);

                if (searchRequest.ToDate.HasValue)
                    query = query.Where(r => r.CreatedAt <= searchRequest.ToDate.Value);

                if (searchRequest.MultitenantId.HasValue)
                    query = query.Where(r => r.MultitenantId == searchRequest.MultitenantId.Value);

                if (!string.IsNullOrWhiteSpace(searchRequest.SortBy))
                {
                    query = searchRequest.SortDescending
                        ? query.OrderByDescending(r => EF.Property<object>(r, searchRequest.SortBy))
                        : query.OrderBy(r => EF.Property<object>(r, searchRequest.SortBy));
                }
                else
                {
                    query = query.OrderByDescending(r => r.CreatedAt);
                }

                var validPageSize = searchRequest.GetValidPageSize();
                var validPage = searchRequest.GetValidPage();

                query = query
                    .Skip((validPage - 1) * validPageSize)
                    .Take(validPageSize);

                var result = await query
                    .Select(r => new RepairDetailDto
                    {
                        Id = r.Id,
                        RepairId = r.RepairId,
                        RepairCode = r.RepairCode,
                        Customer = new CustomerDetailDto
                        {
                            Id = r.CustomerId ?? Guid.Empty,
                            Name = r.Customer != null ?
                                (r.Customer.RagioneSociale ?? $"{r.Customer.Cognome} {r.Customer.Nome}".Trim()) :
                                "Cliente non trovato",
                            Phone = r.Customer != null ? r.Customer.Telefono : null,
                            CustomerType = r.Customer != null && r.Customer.Tipologia == "1" ? "Privato" : "Azienda"
                        },
                        Device = new DeviceDetailDto
                        {
                            DeviceId = r.DeviceId,
                            Brand = r.Device != null ? r.Device.Brand : "Sconosciuto",
                            Model = r.Device != null ? r.Device.Model : "Sconosciuto",
                            SerialNumber = r.Device != null ? r.Device.SerialNumber : null
                        },
                        FaultDeclared = r.FaultDeclared,
                        RepairStatus = r.RepairStatus,
                        RepairStatusCode = r.RepairStatusCode,
                        TechnicianName = r.TechnicianName,
                        CreatedAt = r.CreatedAt,
                        ReceivedAt = r.ReceivedAt,
                        HasDiagnostic = _context.IncomingTests.Any(it => it.RepairId == r.RepairId)
                    })
                    .ToListAsync();

                _logger.LogInformation("✅ Found {RepairCount} light repairs matching criteria", result.Count());


                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving light repairs list");
                throw;
            }
        }

    }
}