using MediaLabAPI.DTOs;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public interface IWarehouseService
    {
        // Metodi principali per gli articoli
        Task<WarehouseItemsPagedResponseDto> GetWarehouseItemsAsync(WarehouseSearchDto searchDto);
        Task<WarehouseItemDetailDto?> GetWarehouseItemByIdAsync(int id);
        Task<WarehouseItemDetailDto?> GetWarehouseItemByGuidAsync(Guid itemId);
        Task<WarehouseItemDetailDto?> GetWarehouseItemByCodeAsync(string code);
        Task<CreateWarehouseItemResponseDto> CreateWarehouseItemAsync(CreateWarehouseItemDto createDto);
        Task UpdateWarehouseItemAsync(int id, UpdateWarehouseItemDto updateDto);
        Task DeleteWarehouseItemAsync(int id);

        // Gestione quantità e movimenti
        Task UpdateQuantityAsync(int id, UpdateQuantityDto updateDto);
        Task RegisterMovementAsync(WarehouseMovementDto movementDto);

        // Statistiche e reporting
        Task<WarehouseStatsDto> GetWarehouseStatsAsync(Guid? multitenantId);
        Task<List<WarehouseItemDetailDto>> GetLowStockItemsAsync(Guid? multitenantId);

        // Ricerca e liste semplificate
        Task<List<WarehouseItemLightDto>> GetWarehouseItemsLightAsync(Guid? multitenantId, string? category);
        Task<List<WarehouseItemLightDto>> QuickSearchAsync(string query, Guid? multitenantId);

        // Gestione categorie
        Task<List<CategoryInfoDto>> GetCategoriesAsync(Guid? multitenantId);
        Task<CategoryInfoDto> CreateCategoryAsync(CreateWarehouseCategoryDto createDto);

        // Gestione fornitori
        Task<List<WarehouseSupplier>> GetSuppliersAsync(Guid? multitenantId);
        Task<WarehouseSupplier> CreateSupplierAsync(CreateWarehouseSupplierDto createDto);

        // Export
        Task<byte[]> ExportToCsvAsync(Guid? multitenantId);
    }
}