using MediaLabAPI.DTOs.DeviceInventory;
using MediaLabAPI.Models;
using MediaLabAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace MediaLabAPI.Services
{
    public class DeviceInventoryService : IDeviceInventoryService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DeviceInventoryService> _logger;

        public DeviceInventoryService(AppDbContext context, ILogger<DeviceInventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==========================================
        // CRUD BASE
        // ==========================================

        public async Task<DeviceInventoryPagedResponseDto> SearchDevicesAsync(DeviceInventorySearchDto searchDto)
        {
            var query = _context.DeviceInventory
                .Where(d => !d.IsDeleted)
                .AsQueryable();

            // FiltriD
            if (searchDto.MultitenantId.HasValue)
            {
                query = query.Where(d => d.MultitenantId == searchDto.MultitenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchQuery))
            {
                var searchTerm = searchDto.SearchQuery.ToLower();
                query = query.Where(d =>
                    d.Code.ToLower().Contains(searchTerm) ||
                    d.Brand.ToLower().Contains(searchTerm) ||
                    d.Model.ToLower().Contains(searchTerm) ||
                    d.IMEI.ToLower().Contains(searchTerm) ||
                    (d.ESN != null && d.ESN.ToLower().Contains(searchTerm)) ||
                    d.Color.ToLower().Contains(searchTerm)
                );
            }

            if (!string.IsNullOrWhiteSpace(searchDto.DeviceType))
            {
                query = query.Where(d => d.DeviceType == searchDto.DeviceType);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Brand))
            {
                query = query.Where(d => d.Brand.ToLower().Contains(searchDto.Brand.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.DeviceCondition))
            {
                query = query.Where(d => d.DeviceCondition == searchDto.DeviceCondition);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.DeviceStatus))
            {
                query = query.Where(d => d.DeviceStatus == searchDto.DeviceStatus);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SupplierId))
            {
                query = query.Where(d => d.SupplierId == searchDto.SupplierId);
            }

            if (searchDto.IsCourtesyDevice.HasValue)
            {
                query = query.Where(d => d.IsCourtesyDevice == searchDto.IsCourtesyDevice.Value);
            }

            if (searchDto.MinPurchasePrice.HasValue)
            {
                query = query.Where(d => d.PurchasePrice >= searchDto.MinPurchasePrice.Value);
            }

            if (searchDto.MaxPurchasePrice.HasValue)
            {
                query = query.Where(d => d.PurchasePrice <= searchDto.MaxPurchasePrice.Value);
            }

            if (searchDto.MinSellingPrice.HasValue)
            {
                query = query.Where(d => d.SellingPrice >= searchDto.MinSellingPrice.Value);
            }

            if (searchDto.MaxSellingPrice.HasValue)
            {
                query = query.Where(d => d.SellingPrice <= searchDto.MaxSellingPrice.Value);
            }

            // Conteggio totale
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = searchDto.SortBy?.ToLower() switch
            {
                "code" => searchDto.SortDescending ? query.OrderByDescending(d => d.Code) : query.OrderBy(d => d.Code),
                "brand" => searchDto.SortDescending ? query.OrderByDescending(d => d.Brand) : query.OrderBy(d => d.Brand),
                "model" => searchDto.SortDescending ? query.OrderByDescending(d => d.Model) : query.OrderBy(d => d.Model),
                "status" => searchDto.SortDescending ? query.OrderByDescending(d => d.DeviceStatus) : query.OrderBy(d => d.DeviceStatus),
                "created" => searchDto.SortDescending ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt),
                _ => query.OrderByDescending(d => d.CreatedAt)
            };

            // Paginazione
            var skip = (searchDto.Page - 1) * searchDto.PageSize;
            var devices = await query
                .Include(d => d.Supplier)
                .Skip(skip)
                .Take(searchDto.PageSize)
                .AsNoTracking()
                .ToListAsync();

            // Mappa i risultati
            var deviceDtos = devices.Select(MapToDto).ToList();

            // Calcola statistiche
            var stats = await GetStatsAsync(searchDto.MultitenantId);

            var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

            return new DeviceInventoryPagedResponseDto
            {
                Items = deviceDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1,
                Stats = stats
            };
        }

        public async Task<DeviceInventoryDto?> GetDeviceByIdAsync(int id)
        {
            var device = await _context.DeviceInventory
                .Include(d => d.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            return device == null ? null : MapToDto(device);
        }

        public async Task<DeviceInventoryDto?> GetDeviceByGuidAsync(Guid deviceId)
        {
            var device = await _context.DeviceInventory
                .Include(d => d.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId && !d.IsDeleted);

            return device == null ? null : MapToDto(device);
        }

        public async Task<DeviceInventoryDto?> GetDeviceByCodeAsync(string code)
        {
            var device = await _context.DeviceInventory
                .Include(d => d.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Code == code && !d.IsDeleted);

            return device == null ? null : MapToDto(device);
        }

        public async Task<DeviceInventoryDto?> GetDeviceByImeiAsync(string imei)
        {
            var device = await _context.DeviceInventory
                .Include(d => d.Supplier)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.IMEI == imei && !d.IsDeleted);

            return device == null ? null : MapToDto(device);
        }

        public async Task<CreateDeviceInventoryResponseDto> CreateDeviceAsync(CreateDeviceInventoryDto createDto)
        {
            // Verifica unicità del codice
            var existingCode = await _context.DeviceInventory
                .AnyAsync(d => d.Code == createDto.Code && !d.IsDeleted);

            if (existingCode)
            {
                throw new ArgumentException($"Esiste già un apparato con codice '{createDto.Code}'");
            }

            // Verifica unicità IMEI
            var existingImei = await _context.DeviceInventory
                .AnyAsync(d => d.IMEI == createDto.IMEI && !d.IsDeleted);

            if (existingImei)
            {
                throw new ArgumentException($"Esiste già un apparato con IMEI '{createDto.IMEI}'");
            }

            // Verifica che il fornitore esista
            var supplierExists = await _context.DeviceInventorySuppliers
                .AnyAsync(s => s.SupplierId == createDto.SupplierId &&
                              s.MultitenantId == createDto.MultitenantId &&
                              !s.IsDeleted);

            if (!supplierExists)
            {
                throw new ArgumentException($"Fornitore '{createDto.SupplierId}' non trovato");
            }

            // Ottiene la CompanyId dal MultitenantId
            var company = await _context.C_ANA_Companies
                .FirstOrDefaultAsync(c => c.Id == createDto.MultitenantId);

            if (company == null)
            {
                throw new ArgumentException("Company non trovata");
            }

            var device = new DeviceInventory
            {
                DeviceId = Guid.NewGuid(),
                Code = createDto.Code,
                DeviceType = createDto.DeviceType,
                Brand = createDto.Brand,
                Model = createDto.Model,
                IMEI = createDto.IMEI,
                ESN = createDto.ESN,
                SerialNumber = createDto.SerialNumber,
                Color = createDto.Color,
                DeviceCondition = createDto.DeviceCondition,
                IsCourtesyDevice = createDto.IsCourtesyDevice,
                DeviceStatus = createDto.DeviceStatus,
                SupplierId = createDto.SupplierId,
                PurchasePrice = createDto.PurchasePrice,
                SellingPrice = createDto.SellingPrice,
                PurchaseDate = createDto.PurchaseDate,
                Location = createDto.Location,
                Notes = createDto.Notes,
                CompanyId = company.Id,
                MultitenantId = createDto.MultitenantId,
                CreatedAt = DateTime.UtcNow
            };

            _context.DeviceInventory.Add(device);
            await _context.SaveChangesAsync();

            // Registra il movimento di acquisto
            await RegisterMovementAsync(device.Id, device.DeviceId, "purchase", null, device.DeviceStatus,
                null, "Acquisto iniziale", device.MultitenantId);

            return new CreateDeviceInventoryResponseDto
            {
                Id = device.Id,
                DeviceId = device.DeviceId,
                Code = device.Code,
                Brand = device.Brand,
                Model = device.Model,
                Message = "Apparato creato con successo",
                CreatedAt = device.CreatedAt
            };
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceInventoryDto updateDto)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            // Verifica unicità del codice (escludendo l'apparato corrente)
            var existingCode = await _context.DeviceInventory
                .AnyAsync(d => d.Code == updateDto.Code && d.Id != id && !d.IsDeleted);

            if (existingCode)
            {
                throw new ArgumentException($"Esiste già un altro apparato con codice '{updateDto.Code}'");
            }

            // Verifica unicità IMEI (escludendo l'apparato corrente)
            var existingImei = await _context.DeviceInventory
                .AnyAsync(d => d.IMEI == updateDto.IMEI && d.Id != id && !d.IsDeleted);

            if (existingImei)
            {
                throw new ArgumentException($"Esiste già un altro apparato con IMEI '{updateDto.IMEI}'");
            }

            // Verifica che il fornitore esista
            var supplierExists = await _context.DeviceInventorySuppliers
                .AnyAsync(s => s.SupplierId == updateDto.SupplierId &&
                              s.MultitenantId == device.MultitenantId &&
                              !s.IsDeleted);

            if (!supplierExists)
            {
                throw new ArgumentException($"Fornitore '{updateDto.SupplierId}' non trovato");
            }

            var oldStatus = device.DeviceStatus;

            device.Code = updateDto.Code;
            device.DeviceType = updateDto.DeviceType;
            device.Brand = updateDto.Brand;
            device.Model = updateDto.Model;
            device.IMEI = updateDto.IMEI;
            device.ESN = updateDto.ESN;
            device.SerialNumber = updateDto.SerialNumber;
            device.Color = updateDto.Color;
            device.DeviceCondition = updateDto.DeviceCondition;
            device.IsCourtesyDevice = updateDto.IsCourtesyDevice;
            device.DeviceStatus = updateDto.DeviceStatus;
            device.SupplierId = updateDto.SupplierId;
            device.PurchasePrice = updateDto.PurchasePrice;
            device.SellingPrice = updateDto.SellingPrice;
            device.PurchaseDate = updateDto.PurchaseDate;
            device.Location = updateDto.Location;
            device.Notes = updateDto.Notes;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Se lo stato è cambiato, registra il movimento
            if (oldStatus != device.DeviceStatus)
            {
                await RegisterMovementAsync(device.Id, device.DeviceId, "status_change",
                    oldStatus, device.DeviceStatus, null, "Aggiornamento apparato", device.MultitenantId);
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            device.IsDeleted = true;
            device.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // STATISTICHE
        // ==========================================

        public async Task<DeviceInventoryStatsDto> GetStatsAsync(Guid? multitenantId)
        {
            var query = _context.DeviceInventory
                .Where(d => !d.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(d => d.MultitenantId == multitenantId.Value);
            }

            var devices = await query.ToListAsync();

            return new DeviceInventoryStatsDto
            {
                TotalDevices = devices.Count,
                AvailableDevices = devices.Count(d => d.DeviceStatus == "available"),
                LoanedDevices = devices.Count(d => d.DeviceStatus == "loaned"),
                SoldDevices = devices.Count(d => d.DeviceStatus == "sold"),
                CourtesyDevices = devices.Count(d => d.IsCourtesyDevice && d.DeviceStatus == "available"),
                Smartphones = devices.Count(d => d.DeviceType == "smartphone"),
                Tablets = devices.Count(d => d.DeviceType == "tablet"),
                TotalPurchaseValue = devices.Sum(d => d.PurchasePrice),
                TotalSellingValue = devices.Sum(d => d.SellingPrice),
                PotentialProfit = devices.Sum(d => d.SellingPrice - d.PurchasePrice),
                NewDevices = devices.Count(d => d.DeviceCondition == "new"),
                UsedDevices = devices.Count(d => d.DeviceCondition == "used"),
                RefurbishedDevices = devices.Count(d => d.DeviceCondition == "refurbished")
            };
        }

        // ==========================================
        // OPERAZIONI SPECIALI
        // ==========================================

        public async Task<List<DeviceInventoryDto>> GetCourtesyAvailableDevicesAsync(Guid? multitenantId)
        {
            var query = _context.DeviceInventory
                .Where(d => !d.IsDeleted &&
                           d.IsCourtesyDevice &&
                           d.DeviceStatus == "available");

            if (multitenantId.HasValue)
            {
                query = query.Where(d => d.MultitenantId == multitenantId.Value);
            }

            var devices = await query
                .Include(d => d.Supplier)
                .OrderBy(d => d.Brand)
                .ThenBy(d => d.Model)
                .AsNoTracking()
                .ToListAsync();

            return devices.Select(MapToDto).ToList();
        }

        public async Task ChangeDeviceStatusAsync(int id, ChangeDeviceStatusDto statusDto)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            var oldStatus = device.DeviceStatus;
            device.DeviceStatus = statusDto.NewStatus;
            device.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(statusDto.Notes))
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var statusNote = $"[{timestamp}] Cambio stato: {oldStatus} → {statusDto.NewStatus} - {statusDto.Notes}";
                device.Notes = string.IsNullOrWhiteSpace(device.Notes)
                    ? statusNote
                    : $"{device.Notes}\n{statusNote}";
            }

            await _context.SaveChangesAsync();

            // Registra il movimento
            await RegisterMovementAsync(device.Id, device.DeviceId, "status_change",
                oldStatus, statusDto.NewStatus, null, statusDto.Notes, device.MultitenantId);
        }

        public async Task LoanDeviceAsync(int id, LoanDeviceDto loanDto)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            if (device.DeviceStatus != "available")
            {
                throw new ArgumentException($"L'apparato non è disponibile (stato attuale: {device.DeviceStatus})");
            }

            // Verifica che il cliente esista
            var customerExists = await _context.C_ANA_Companies
                .AnyAsync(c => c.Id == loanDto.CustomerId);

            if (!customerExists)
            {
                throw new ArgumentException("Cliente non trovato");
            }

            var oldStatus = device.DeviceStatus;
            device.DeviceStatus = "loaned";
            device.UpdatedAt = DateTime.UtcNow;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var loanNote = $"[{timestamp}] In prestito a cliente {loanDto.CustomerId}";
            if (!string.IsNullOrWhiteSpace(loanDto.Reference))
            {
                loanNote += $" (Rif: {loanDto.Reference})";
            }
            if (loanDto.ExpectedReturnDate.HasValue)
            {
                loanNote += $" - Rientro previsto: {loanDto.ExpectedReturnDate.Value:dd/MM/yyyy}";
            }
            if (!string.IsNullOrWhiteSpace(loanDto.Notes))
            {
                loanNote += $" - {loanDto.Notes}";
            }

            device.Notes = string.IsNullOrWhiteSpace(device.Notes)
                ? loanNote
                : $"{device.Notes}\n{loanNote}";

            await _context.SaveChangesAsync();

            // Registra il movimento
            await RegisterMovementAsync(device.Id, device.DeviceId, "loan",
                oldStatus, "loaned", loanDto.CustomerId, loanDto.Notes, device.MultitenantId, loanDto.Reference);
        }

        public async Task ReturnDeviceAsync(int id, ReturnDeviceDto returnDto)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            if (device.DeviceStatus != "loaned")
            {
                throw new ArgumentException($"L'apparato non è in prestito (stato attuale: {device.DeviceStatus})");
            }

            var oldStatus = device.DeviceStatus;
            device.DeviceStatus = returnDto.ReturnStatus;
            device.UpdatedAt = DateTime.UtcNow;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            var returnNote = $"[{timestamp}] Restituito - Nuovo stato: {returnDto.ReturnStatus}";
            if (!string.IsNullOrWhiteSpace(returnDto.Condition))
            {
                returnNote += $" - Condizioni: {returnDto.Condition}";
            }
            if (!string.IsNullOrWhiteSpace(returnDto.Notes))
            {
                returnNote += $" - {returnDto.Notes}";
            }

            device.Notes = string.IsNullOrWhiteSpace(device.Notes)
                ? returnNote
                : $"{device.Notes}\n{returnNote}";

            await _context.SaveChangesAsync();

            // Registra il movimento
            await RegisterMovementAsync(device.Id, device.DeviceId, "return",
                oldStatus, returnDto.ReturnStatus, null, returnDto.Notes, device.MultitenantId);
        }

        // ==========================================
        // MOVIMENTI
        // ==========================================

        public async Task<List<DeviceInventoryMovementDto>> GetDeviceMovementsAsync(int id)
        {
            var device = await _context.DeviceInventory
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

            if (device == null)
            {
                throw new ArgumentException("Apparato non trovato");
            }

            var movements = await _context.DeviceInventoryMovements
                .Where(m => m.DeviceInventoryId == id)
                .Include(m => m.Customer)
                .OrderByDescending(m => m.MovementDate)
                .AsNoTracking()
                .ToListAsync();

            return movements.Select(m => new DeviceInventoryMovementDto
            {
                Id = m.Id,
                MovementId = m.MovementId,
                MovementType = m.MovementType,
                FromStatus = m.FromStatus,
                ToStatus = m.ToStatus,
                CustomerId = m.CustomerId,
                CustomerName = m.Customer != null
                    ? (string.IsNullOrWhiteSpace(m.Customer.RagioneSociale)
                        ? $"{m.Customer.Nome} {m.Customer.Cognome}"
                        : m.Customer.RagioneSociale)
                    : null,
                Reference = m.Reference,
                Notes = m.Notes,
                MovementDate = m.MovementDate,
                CreatedBy = m.CreatedBy
            }).ToList();
        }

        private async Task RegisterMovementAsync(int deviceInventoryId, Guid deviceId, string movementType,
            string? fromStatus, string? toStatus, Guid? customerId, string? notes, Guid multitenantId, string? reference = null)
        {
            var device = await _context.DeviceInventory.FindAsync(deviceInventoryId);
            if (device == null) return;

            var movement = new DeviceInventoryMovement
            {
                MovementId = Guid.NewGuid(),
                DeviceInventoryId = deviceInventoryId,
                DeviceId = deviceId,
                MovementType = movementType,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                CustomerId = customerId,
                Reference = reference,
                Notes = notes,
                CompanyId = device.CompanyId,
                MultitenantId = multitenantId,
                MovementDate = DateTime.UtcNow
            };

            _context.DeviceInventoryMovements.Add(movement);
            await _context.SaveChangesAsync();
        }

        // ==========================================
        // FORNITORI
        // ==========================================

        public async Task<List<DeviceInventorySupplierDto>> GetSuppliersAsync(Guid? multitenantId)
        {
            var query = _context.DeviceInventorySuppliers
                .Where(s => !s.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(s => s.MultitenantId == multitenantId.Value);
            }

            var suppliers = await query
                .Include(s => s.Devices.Where(d => !d.IsDeleted))
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();

            return suppliers.Select(s => new DeviceInventorySupplierDto
            {
                Id = s.Id,
                SupplierId = s.SupplierId,
                Name = s.Name,
                Contact = s.Contact,
                Phone = s.Phone,
                Email = s.Email,
                DeviceCount = s.Devices.Count
            }).ToList();
        }

        public async Task<DeviceInventorySupplierDto> CreateSupplierAsync(CreateDeviceInventorySupplierDto createDto)
        {
            // Verifica che non esista già
            var existing = await _context.DeviceInventorySuppliers
                .AnyAsync(s => s.SupplierId == createDto.SupplierId &&
                              s.MultitenantId == createDto.MultitenantId &&
                              !s.IsDeleted);

            if (existing)
            {
                throw new ArgumentException($"Esiste già un fornitore con ID '{createDto.SupplierId}'");
            }

            var company = await _context.C_ANA_Companies
                .FirstOrDefaultAsync(c => c.Id == createDto.MultitenantId);

            if (company == null)
            {
                throw new ArgumentException("Company non trovata");
            }

            var supplier = new DeviceInventorySupplier
            {
                SupplierId = createDto.SupplierId,
                Name = createDto.Name,
                Contact = createDto.Contact,
                Address = createDto.Address,
                Phone = createDto.Phone,
                Email = createDto.Email,
                Notes = createDto.Notes,
                CompanyId = company.Id,
                MultitenantId = createDto.MultitenantId,
                CreatedAt = DateTime.UtcNow
            };

            _context.DeviceInventorySuppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return new DeviceInventorySupplierDto
            {
                Id = supplier.Id,
                SupplierId = supplier.SupplierId,
                Name = supplier.Name,
                Contact = supplier.Contact,
                Phone = supplier.Phone,
                Email = supplier.Email,
                DeviceCount = 0
            };
        }

        // ==========================================
        // EXPORT
        // ==========================================

        public async Task<byte[]> ExportToCsvAsync(Guid? multitenantId)
        {
            var query = _context.DeviceInventory
                .Where(d => !d.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(d => d.MultitenantId == multitenantId.Value);
            }

            var devices = await query
                .Include(d => d.Supplier)
                .OrderBy(d => d.Code)
                .AsNoTracking()
                .ToListAsync();

            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Codice,Tipo,Marca,Modello,IMEI,ESN,Colore,Condizione,Cortesia,Stato,Fornitore,Prezzo Acquisto,Prezzo Vendita,Ubicazione,Note,Data Creazione");

            // Data
            foreach (var device in devices)
            {
                csv.AppendLine($"\"{device.Code}\"," +
                             $"\"{device.DeviceType}\"," +
                             $"\"{device.Brand}\"," +
                             $"\"{device.Model}\"," +
                             $"\"{device.IMEI}\"," +
                             $"\"{device.ESN ?? ""}\"," +
                             $"\"{device.Color}\"," +
                             $"\"{device.DeviceCondition}\"," +
                             $"\"{(device.IsCourtesyDevice ? "Sì" : "No")}\"," +
                             $"\"{device.DeviceStatus}\"," +
                             $"\"{device.Supplier?.Name ?? device.SupplierId}\"," +
                             $"\"{device.PurchasePrice.ToString("F2", CultureInfo.InvariantCulture)}\"," +
                             $"\"{device.SellingPrice.ToString("F2", CultureInfo.InvariantCulture)}\"," +
                             $"\"{device.Location ?? ""}\"," +
                             $"\"{(device.Notes ?? "").Replace("\"", "\"\"")}\"," +
                             $"\"{device.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // ==========================================
        // METODI PRIVATI DI UTILITÀ
        // ==========================================

        private DeviceInventoryDto MapToDto(DeviceInventory device)
        {
            return new DeviceInventoryDto
            {
                Id = device.Id,
                DeviceId = device.DeviceId,
                Code = device.Code,
                DeviceType = device.DeviceType,
                Brand = device.Brand,
                Model = device.Model,
                IMEI = device.IMEI,
                ESN = device.ESN,
                SerialNumber = device.SerialNumber,
                Color = device.Color,
                DeviceCondition = device.DeviceCondition,
                IsCourtesyDevice = device.IsCourtesyDevice,
                DeviceStatus = device.DeviceStatus,
                SupplierId = device.SupplierId,
                SupplierName = device.Supplier?.Name,
                PurchasePrice = device.PurchasePrice,
                SellingPrice = device.SellingPrice,
                PurchaseDate = device.PurchaseDate,
                Location = device.Location,
                Notes = device.Notes,
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt,
                CreatedBy = device.CreatedBy,
                UpdatedBy = device.UpdatedBy
            };
        }
    }
}