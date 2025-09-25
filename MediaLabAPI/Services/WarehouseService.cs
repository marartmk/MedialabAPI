using MediaLabAPI.DTOs;
using MediaLabAPI.Models;
using MediaLabAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace MediaLabAPI.Services
{

    public class WarehouseService : IWarehouseService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WarehouseService> _logger;

        public WarehouseService(AppDbContext context, ILogger<WarehouseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WarehouseItemsPagedResponseDto> GetWarehouseItemsAsync(WarehouseSearchDto searchDto)
        {
            var query = _context.WarehouseItems
                .Where(w => !w.IsDeleted)
                .AsQueryable();

            // Filtri
            if (searchDto.MultitenantId.HasValue)
            {
                query = query.Where(w => w.MultitenantId == searchDto.MultitenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.SearchQuery))
            {
                var searchTerm = searchDto.SearchQuery.ToLower();
                query = query.Where(w =>
                    w.Name.ToLower().Contains(searchTerm) ||
                    w.Code.ToLower().Contains(searchTerm) ||
                    w.Brand.ToLower().Contains(searchTerm) ||
                    w.Model.ToLower().Contains(searchTerm) ||
                    (w.Description != null && w.Description.ToLower().Contains(searchTerm))
                );
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Category))
            {
                query = query.Where(w => w.Category == searchDto.Category);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Supplier))
            {
                query = query.Where(w => w.Supplier == searchDto.Supplier);
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Brand))
            {
                query = query.Where(w => w.Brand.ToLower().Contains(searchDto.Brand.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.StockStatus))
            {
                query = searchDto.StockStatus.ToLower() switch
                {
                    "available" => query.Where(w => w.Quantity > w.MinQuantity),
                    "low" => query.Where(w => w.Quantity > 0 && w.Quantity <= w.MinQuantity),
                    "out" => query.Where(w => w.Quantity == 0),
                    _ => query
                };
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(w => w.UnitPrice >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(w => w.UnitPrice <= searchDto.MaxPrice.Value);
            }

            // Conteggio totale
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = searchDto.SortBy?.ToLower() switch
            {
                "name" => searchDto.SortDescending ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
                "code" => searchDto.SortDescending ? query.OrderByDescending(w => w.Code) : query.OrderBy(w => w.Code),
                "quantity" => searchDto.SortDescending ? query.OrderByDescending(w => w.Quantity) : query.OrderBy(w => w.Quantity),
                "price" => searchDto.SortDescending ? query.OrderByDescending(w => w.UnitPrice) : query.OrderBy(w => w.UnitPrice),
                "brand" => searchDto.SortDescending ? query.OrderByDescending(w => w.Brand) : query.OrderBy(w => w.Brand),
                "category" => searchDto.SortDescending ? query.OrderByDescending(w => w.Category) : query.OrderBy(w => w.Category),
                "created" => searchDto.SortDescending ? query.OrderByDescending(w => w.CreatedAt) : query.OrderBy(w => w.CreatedAt),
                _ => query.OrderBy(w => w.Name)
            };

            // Paginazione
            var skip = (searchDto.Page - 1) * searchDto.PageSize;
            var items = await query
                .Include(w => w.Company)  // ✅ SPOSTA INCLUDE PRIMA
                .Skip(skip)
                .Take(searchDto.PageSize)
                .ToListAsync();

            // Carica le categorie e i fornitori
            var categories = await GetCategoriesAsync(searchDto.MultitenantId);
            var suppliers = await GetSuppliersAsync(searchDto.MultitenantId);

            // Mappo gli items
            var itemDtos = items.Select(item => MapToDetailDto(item, categories, suppliers)).ToList();

            // Calcolo statistiche
            var stats = await GetWarehouseStatsAsync(searchDto.MultitenantId);

            var totalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize);

            return new WarehouseItemsPagedResponseDto
            {
                Items = itemDtos,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = totalPages,
                HasNextPage = searchDto.Page < totalPages,
                HasPreviousPage = searchDto.Page > 1,
                Stats = stats
            };
        }

        public async Task<WarehouseItemDetailDto?> GetWarehouseItemByIdAsync(int id)
        {
            var item = await _context.WarehouseItems
                .Include(w => w.Company)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

            if (item == null) return null;

            var categories = await GetCategoriesAsync(item.MultitenantId);
            var suppliers = await GetSuppliersAsync(item.MultitenantId);

            return MapToDetailDto(item, categories, suppliers);
        }

        public async Task<WarehouseItemDetailDto?> GetWarehouseItemByGuidAsync(Guid itemId)
        {
            var item = await _context.WarehouseItems
                .Include(w => w.Company)
                .FirstOrDefaultAsync(w => w.ItemId == itemId && !w.IsDeleted);

            if (item == null) return null;

            var categories = await GetCategoriesAsync(item.MultitenantId);
            var suppliers = await GetSuppliersAsync(item.MultitenantId);

            return MapToDetailDto(item, categories, suppliers);
        }

        public async Task<WarehouseItemDetailDto?> GetWarehouseItemByCodeAsync(string code)
        {
            var item = await _context.WarehouseItems
                .Include(w => w.Company)
                .FirstOrDefaultAsync(w => w.Code == code && !w.IsDeleted);

            if (item == null) return null;

            var categories = await GetCategoriesAsync(item.MultitenantId);
            var suppliers = await GetSuppliersAsync(item.MultitenantId);

            return MapToDetailDto(item, categories, suppliers);
        }

        public async Task<CreateWarehouseItemResponseDto> CreateWarehouseItemAsync(CreateWarehouseItemDto createDto)
        {
            // Verifica che il codice non esista già
            var existingItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Code == createDto.Code && !w.IsDeleted);

            if (existingItem != null)
            {
                throw new ArgumentException($"Un articolo con codice '{createDto.Code}' esiste già");
            }

            // Ottiene la CompanyId dal MultitenantId (assumendo che ci sia una relazione)
            var company = await _context.C_ANA_Companies
                .FirstOrDefaultAsync(c => c.Id == createDto.MultitenantId);

            if (company == null)
            {
                throw new ArgumentException("Company non trovata per il MultitenantId specificato");
            }

            var item = new WarehouseItem
            {
                ItemId = Guid.NewGuid(),
                Code = createDto.Code,
                Name = createDto.Name,
                Description = createDto.Description,
                Category = createDto.Category,
                Subcategory = createDto.Subcategory,
                Brand = createDto.Brand,
                Model = createDto.Model,
                Supplier = createDto.Supplier,
                Quantity = createDto.Quantity,
                MinQuantity = createDto.MinQuantity,
                MaxQuantity = createDto.MaxQuantity,
                UnitPrice = createDto.UnitPrice,
                TotalValue = createDto.Quantity * createDto.UnitPrice,
                Location = createDto.Location,
                Notes = createDto.Notes,
                CompanyId = company.Id,
                MultitenantId = createDto.MultitenantId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WarehouseItems.Add(item);
            await _context.SaveChangesAsync();

            return new CreateWarehouseItemResponseDto
            {
                Id = item.Id,
                ItemId = item.ItemId,
                Code = item.Code,
                Name = item.Name,
                Message = "Articolo creato con successo",
                CreatedAt = item.CreatedAt
            };
        }

        public async Task UpdateWarehouseItemAsync(int id, UpdateWarehouseItemDto updateDto)
        {
            var item = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

            if (item == null)
            {
                throw new ArgumentException("Articolo non trovato");
            }

            // Verifica che il codice non sia già usato da un altro articolo
            var existingItem = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Code == updateDto.Code && w.Id != id && !w.IsDeleted);

            if (existingItem != null)
            {
                throw new ArgumentException($"Un altro articolo con codice '{updateDto.Code}' esiste già");
            }

            item.Code = updateDto.Code;
            item.Name = updateDto.Name;
            item.Description = updateDto.Description;
            item.Category = updateDto.Category;
            item.Subcategory = updateDto.Subcategory;
            item.Brand = updateDto.Brand;
            item.Model = updateDto.Model;
            item.Supplier = updateDto.Supplier;
            item.Quantity = updateDto.Quantity;
            item.MinQuantity = updateDto.MinQuantity;
            item.MaxQuantity = updateDto.MaxQuantity;
            item.UnitPrice = updateDto.UnitPrice;
            item.TotalValue = updateDto.Quantity * updateDto.UnitPrice;
            item.Location = updateDto.Location;
            item.Notes = updateDto.Notes;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteWarehouseItemAsync(int id)
        {
            var item = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

            if (item == null)
            {
                throw new ArgumentException("Articolo non trovato");
            }

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int id, UpdateQuantityDto updateDto)
        {
            var item = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

            if (item == null)
            {
                throw new ArgumentException("Articolo non trovato");
            }

            var oldQuantity = item.Quantity;

            item.Quantity = updateDto.Action.ToLower() switch
            {
                "add" => item.Quantity + updateDto.Quantity,
                "remove" => Math.Max(0, item.Quantity - updateDto.Quantity),
                "set" => updateDto.Quantity,
                _ => throw new ArgumentException("Azione non valida. Usa 'add', 'remove' o 'set'")
            };

            item.TotalValue = item.Quantity * item.UnitPrice;
            item.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(updateDto.Notes))
            {
                var currentNotes = item.Notes ?? string.Empty;
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var movementNote = $"[{timestamp}] Quantità: {oldQuantity} → {item.Quantity} ({updateDto.Action}): {updateDto.Notes}";

                item.Notes = string.IsNullOrWhiteSpace(currentNotes)
                    ? movementNote
                    : $"{currentNotes}\n{movementNote}";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<WarehouseStatsDto> GetWarehouseStatsAsync(Guid? multitenantId)
        {
            var query = _context.WarehouseItems
                .Where(w => !w.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(w => w.MultitenantId == multitenantId.Value);
            }

            var items = await query.ToListAsync();

            var stats = new WarehouseStatsDto
            {
                TotalItems = items.Count,
                AvailableItems = items.Count(i => i.Quantity > i.MinQuantity),
                LowStockItems = items.Count(i => i.Quantity > 0 && i.Quantity <= i.MinQuantity),
                OutOfStockItems = items.Count(i => i.Quantity == 0),
                TotalValue = items.Sum(i => i.TotalValue),
                TotalCategories = items.Select(i => i.Category).Distinct().Count(),
                TotalSuppliers = items.Select(i => i.Supplier).Distinct().Count()
            };

            return stats;
        }

        public async Task<List<WarehouseItemDetailDto>> GetLowStockItemsAsync(Guid? multitenantId)
        {
            IQueryable<WarehouseItem> query = _context.WarehouseItems
                .Where(w => !w.IsDeleted && w.Quantity > 0 && w.Quantity <= w.MinQuantity);

            if (multitenantId.HasValue)
            {
                query = query.Where(w => w.MultitenantId == multitenantId.Value);
            }

            // Include dopo i filtri per evitare il problema di cast
            var itemsTask = query
                .Include(w => w.Company)
                .OrderBy(w => w.Quantity)
                .AsNoTracking()
                .ToListAsync();

            // In parallelo per performance
            var categoriesTask = GetCategoriesAsync(multitenantId);
            var suppliersTask = GetSuppliersAsync(multitenantId);

            await Task.WhenAll(itemsTask, categoriesTask, suppliersTask);

            var items = itemsTask.Result;
            var categories = categoriesTask.Result;
            var suppliers = suppliersTask.Result;

            return items.Select(item => MapToDetailDto(item, categories, suppliers)).ToList();
        }


        public async Task<List<WarehouseItemLightDto>> GetWarehouseItemsLightAsync(Guid? multitenantId, string? category)
        {
            var query = _context.WarehouseItems
                .Where(w => !w.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(w => w.MultitenantId == multitenantId.Value);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(w => w.Category == category);
            }

            var items = await query
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseItemLightDto
                {
                    Id = w.Id,
                    ItemId = w.ItemId,
                    Code = w.Code,
                    Name = w.Name,
                    Brand = w.Brand,
                    Model = w.Model,
                    Quantity = w.Quantity,
                    UnitPrice = w.UnitPrice,
                    StockStatus = w.Quantity == 0 ? "out" : (w.Quantity <= w.MinQuantity ? "low" : "available")
                })
                .ToListAsync();

            return items;
        }

        public async Task<List<WarehouseItemLightDto>> QuickSearchAsync(string query, Guid? multitenantId)
        {
            var searchTerm = query.ToLower();

            var itemsQuery = _context.WarehouseItems
                .Where(w => !w.IsDeleted && (
                    w.Name.ToLower().Contains(searchTerm) ||
                    w.Code.ToLower().Contains(searchTerm) ||
                    w.Brand.ToLower().Contains(searchTerm) ||
                    w.Model.ToLower().Contains(searchTerm)
                ));

            if (multitenantId.HasValue)
            {
                itemsQuery = itemsQuery.Where(w => w.MultitenantId == multitenantId.Value);
            }

            var items = await itemsQuery
                .Take(10) // Limita i risultati per ricerca rapida
                .Select(w => new WarehouseItemLightDto
                {
                    Id = w.Id,
                    ItemId = w.ItemId,
                    Code = w.Code,
                    Name = w.Name,
                    Brand = w.Brand,
                    Model = w.Model,
                    Quantity = w.Quantity,
                    UnitPrice = w.UnitPrice,
                    StockStatus = w.Quantity == 0 ? "out" : (w.Quantity <= w.MinQuantity ? "low" : "available")
                })
                .ToListAsync();

            return items;
        }

        public async Task RegisterMovementAsync(WarehouseMovementDto movementDto)
        {
            var item = await _context.WarehouseItems
                .FirstOrDefaultAsync(w => w.ItemId == movementDto.ItemId && !w.IsDeleted);

            if (item == null)
            {
                throw new ArgumentException("Articolo non trovato");
            }

            var oldQuantity = item.Quantity;

            // Aggiorna la quantità in base al tipo di movimento
            item.Quantity = movementDto.MovementType.ToLower() switch
            {
                "in" => item.Quantity + movementDto.Quantity,
                "out" => Math.Max(0, item.Quantity - movementDto.Quantity),
                "adjustment" => movementDto.Quantity,
                _ => throw new ArgumentException("Tipo movimento non valido. Usa 'in', 'out' o 'adjustment'")
            };

            item.TotalValue = item.Quantity * item.UnitPrice;
            item.UpdatedAt = DateTime.UtcNow;

            // Aggiunge nota del movimento
            var timestamp = (movementDto.Date ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm");
            var movementNote = $"[{timestamp}] Movimento {movementDto.MovementType}: {oldQuantity} → {item.Quantity}";

            if (!string.IsNullOrWhiteSpace(movementDto.Reference))
            {
                movementNote += $" (Rif: {movementDto.Reference})";
            }

            if (!string.IsNullOrWhiteSpace(movementDto.Notes))
            {
                movementNote += $" - {movementDto.Notes}";
            }

            var currentNotes = item.Notes ?? string.Empty;
            item.Notes = string.IsNullOrWhiteSpace(currentNotes)
                ? movementNote
                : $"{currentNotes}\n{movementNote}";

            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryInfoDto>> GetCategoriesAsync(Guid? multitenantId)
        {
            var query = _context.WarehouseCategories
                .Where(c => !c.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(c => c.MultitenantId == multitenantId.Value);
            }

            var categories = await query
                .OrderBy(c => c.Name)
                .Select(c => new CategoryInfoDto
                {
                    Id = c.CategoryId,
                    Name = c.Name,
                    Icon = c.Icon,
                    Color = c.Color
                })
                .ToListAsync();

            // Se non ci sono categorie nel database, restituisce quelle di default
            if (!categories.Any())
            {
                return GetDefaultCategories();
            }

            return categories;
        }

        public async Task<CategoryInfoDto> CreateCategoryAsync(CreateWarehouseCategoryDto createDto)
        {
            // Verifica che non esista già
            var existing = await _context.WarehouseCategories
                .FirstOrDefaultAsync(c => c.CategoryId == createDto.CategoryId &&
                                         c.MultitenantId == createDto.MultitenantId &&
                                         !c.IsDeleted);

            if (existing != null)
            {
                throw new ArgumentException("Una categoria con questo ID esiste già");
            }

            var company = await _context.C_ANA_Companies
                .FirstOrDefaultAsync(c => c.Id == createDto.MultitenantId);

            if (company == null)
            {
                throw new ArgumentException("Company non trovata");
            }

            var category = new WarehouseCategory
            {
                CategoryId = createDto.CategoryId,
                Name = createDto.Name,
                Icon = createDto.Icon,
                Color = createDto.Color,
                CompanyId = company.Id,
                MultitenantId = createDto.MultitenantId,
                CreatedAt = DateTime.UtcNow
            };

            _context.WarehouseCategories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryInfoDto
            {
                Id = category.CategoryId,
                Name = category.Name,
                Icon = category.Icon,
                Color = category.Color
            };
        }

        public async Task<List<WarehouseSupplier>> GetSuppliersAsync(Guid? multitenantId)
        {
            var query = _context.WarehouseSuppliers
                .Where(s => !s.IsDeleted);

            if (multitenantId.HasValue)
            {
                query = query.Where(s => s.MultitenantId == multitenantId.Value);
            }

            return await query.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<WarehouseSupplier> CreateSupplierAsync(CreateWarehouseSupplierDto createDto)
        {
            // Verifica che non esista già
            var existing = await _context.WarehouseSuppliers
                .FirstOrDefaultAsync(s => s.SupplierId == createDto.SupplierId &&
                                         s.MultitenantId == createDto.MultitenantId &&
                                         !s.IsDeleted);

            if (existing != null)
            {
                throw new ArgumentException("Un fornitore con questo ID esiste già");
            }

            var company = await _context.C_ANA_Companies
                .FirstOrDefaultAsync(c => c.Id == createDto.MultitenantId);

            if (company == null)
            {
                throw new ArgumentException("Company non trovata");
            }

            var supplier = new WarehouseSupplier
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

            _context.WarehouseSuppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return supplier;
        }

        public async Task<byte[]> ExportToCsvAsync(Guid? multitenantId)
        {
            IQueryable<WarehouseItem> baseQuery = _context.WarehouseItems
                .Where(w => !w.IsDeleted);

            if (multitenantId.HasValue)
            {
                baseQuery = baseQuery.Where(w => w.MultitenantId == multitenantId.Value);
            }

            // Include dopo i filtri + no tracking
            var itemsTask = baseQuery
                .Include(w => w.Company)
                .OrderBy(w => w.Code)
                .AsNoTracking()
                .ToListAsync();

            // In parallelo per performance
            var categoriesTask = GetCategoriesAsync(multitenantId);
            var suppliersTask = GetSuppliersAsync(multitenantId);

            await Task.WhenAll(itemsTask, categoriesTask, suppliersTask);

            var items = itemsTask.Result;
            var categories = categoriesTask.Result;
            var suppliers = suppliersTask.Result;

            var csv = new StringBuilder();

            // Header
            csv.AppendLine("Codice,Nome,Descrizione,Categoria,Sottocategoria,Marca,Modello,Fornitore,Quantità,Quantità Min,Quantità Max,Prezzo Unitario,Valore Totale,Ubicazione,Stato Scorte,Note,Data Creazione");

            // Data
            foreach (var item in items)
            {
                var category = categories.FirstOrDefault(c => c.Id == item.Category);
                var supplier = suppliers.FirstOrDefault(s => s.SupplierId == item.Supplier);
                var stockStatus = GetStockStatus(item);

                csv.AppendLine($"\"{item.Code}\"," +
                             $"\"{item.Name}\"," +
                             $"\"{item.Description ?? ""}\"," +
                             $"\"{category?.Name ?? item.Category}\"," +
                             $"\"{item.Subcategory ?? ""}\"," +
                             $"\"{item.Brand}\"," +
                             $"\"{item.Model}\"," +
                             $"\"{supplier?.Name ?? item.Supplier}\"," +
                             $"{item.Quantity}," +
                             $"{item.MinQuantity}," +
                             $"{item.MaxQuantity}," +
                             $"\"{item.UnitPrice.ToString("F2", CultureInfo.InvariantCulture)}\"," +
                             $"\"{item.TotalValue.ToString("F2", CultureInfo.InvariantCulture)}\"," +
                             $"\"{item.Location ?? ""}\"," +
                             $"\"{GetStockStatusText(stockStatus)}\"," +
                             $"\"{(item.Notes ?? "").Replace("\"", "\"\"")}\"," +
                             $"\"{item.CreatedAt:yyyy-MM-dd HH:mm:ss}\"");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        // Metodi privati di utilità

        private WarehouseItemDetailDto MapToDetailDto(WarehouseItem item, List<CategoryInfoDto> categories, List<WarehouseSupplier> suppliers)
        {
            var category = categories.FirstOrDefault(c => c.Id == item.Category);
            var supplier = suppliers.FirstOrDefault(s => s.SupplierId == item.Supplier);
            var stockStatus = GetStockStatus(item);

            return new WarehouseItemDetailDto
            {
                Id = item.Id,
                ItemId = item.ItemId,
                Code = item.Code,
                Name = item.Name,
                Description = item.Description,
                Category = item.Category,
                Subcategory = item.Subcategory,
                Brand = item.Brand,
                Model = item.Model,
                Supplier = item.Supplier,
                SupplierName = supplier?.Name,
                Quantity = item.Quantity,
                MinQuantity = item.MinQuantity,
                MaxQuantity = item.MaxQuantity,
                UnitPrice = item.UnitPrice,
                TotalValue = item.TotalValue,
                Location = item.Location,
                Notes = item.Notes,
                StockStatus = stockStatus,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                CreatedBy = item.CreatedBy,
                UpdatedBy = item.UpdatedBy,
                CategoryInfo = category
            };
        }

        private string GetStockStatus(WarehouseItem item)
        {
            if (item.Quantity == 0) return "out";
            if (item.Quantity <= item.MinQuantity) return "low";
            return "available";
        }

        private string GetStockStatusText(string status)
        {
            return status switch
            {
                "available" => "Disponibile",
                "low" => "In Esaurimento",
                "out" => "Esaurito",
                _ => "Sconosciuto"
            };
        }

        private List<CategoryInfoDto> GetDefaultCategories()
        {
            return new List<CategoryInfoDto>
            {
                new() { Id = "screens", Name = "Schermi", Icon = "📱", Color = "#17a2b8" },
                new() { Id = "batteries", Name = "Batterie", Icon = "🔋", Color = "#28a745" },
                new() { Id = "cameras", Name = "Fotocamere", Icon = "📷", Color = "#6f42c1" },
                new() { Id = "speakers", Name = "Speaker", Icon = "🔊", Color = "#fd7e14" },
                new() { Id = "motherboards", Name = "Schede Madri", Icon = "💾", Color = "#dc3545" },
                new() { Id = "connectors", Name = "Connettori", Icon = "🔌", Color = "#20c997" },
                new() { Id = "buttons", Name = "Pulsanti", Icon = "⚫", Color = "#6c757d" },
                new() { Id = "chassis", Name = "Telai", Icon = "🔧", Color = "#ffc107" },
                new() { Id = "other", Name = "Altri", Icon = "📦", Color = "#6c757d" }
            };
        }
    }
}