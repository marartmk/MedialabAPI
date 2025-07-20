using MediaLabAPI.DTOs;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public interface IRepairService
    {
        Task<CreateRepairResponseDto> CreateRepairAsync(CreateRepairRequestDto request);
        Task<DeviceRepair?> GetRepairByIdAsync(int id);
        Task<DeviceRepair?> GetRepairByCodeAsync(string repairCode);
        Task<IEnumerable<DeviceRepair>> GetRepairsAsync(Guid? multitenantId, string? status);
        Task<IEnumerable<DeviceRepair>> GetRepairsByCustomerAsync(Guid customerId);
        Task<IEnumerable<DeviceRepair>> GetRepairsByDeviceAsync(int deviceId);
        Task UpdateRepairStatusAsync(int repairId, string statusCode, string status, string? notes);        
        Task UpdateRepairAsync(Guid repairId, UpdateRepairRequestDto request);
        Task<IEnumerable<RepairDetailDto>> GetRepairsLightAsync(RepairSearchRequestDto searchRequest);
    }
}