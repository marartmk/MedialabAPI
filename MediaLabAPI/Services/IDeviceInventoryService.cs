using MediaLabAPI.DTOs;
using MediaLabAPI.DTOs.DeviceInventory;

namespace MediaLabAPI.Services
{
    public interface IDeviceInventoryService
    {
        // CRUD Base
        Task<DeviceInventoryPagedResponseDto> SearchDevicesAsync(DeviceInventorySearchDto searchDto);
        Task<DeviceInventoryDto?> GetDeviceByIdAsync(int id);
        Task<DeviceInventoryDto?> GetDeviceByGuidAsync(Guid deviceId);
        Task<DeviceInventoryDto?> GetDeviceByCodeAsync(string code);
        Task<DeviceInventoryDto?> GetDeviceByImeiAsync(string imei);
        Task<CreateDeviceInventoryResponseDto> CreateDeviceAsync(CreateDeviceInventoryDto createDto);
        Task UpdateDeviceAsync(int id, UpdateDeviceInventoryDto updateDto);
        Task DeleteDeviceAsync(int id);

        // Statistiche
        Task<DeviceInventoryStatsDto> GetStatsAsync(Guid? multitenantId);

        // Operazioni speciali
        Task<List<DeviceInventoryDto>> GetCourtesyAvailableDevicesAsync(Guid? multitenantId);
        Task ChangeDeviceStatusAsync(int id, ChangeDeviceStatusDto statusDto);
        Task LoanDeviceAsync(int id, LoanDeviceDto loanDto);
        Task ReturnDeviceAsync(int id, ReturnDeviceDto returnDto);

        // Movimenti
        Task<List<DeviceInventoryMovementDto>> GetDeviceMovementsAsync(int id);

        // Fornitori
        Task<List<DeviceInventorySupplierDto>> GetSuppliersAsync(Guid? multitenantId);
        Task<DeviceInventorySupplierDto> CreateSupplierAsync(CreateDeviceInventorySupplierDto createDto);

        // Export
        Task<byte[]> ExportToCsvAsync(Guid? multitenantId);
    }
}