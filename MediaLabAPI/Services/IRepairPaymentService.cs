using MediaLabAPI.DTOs;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public interface IRepairPaymentService
    {
        // CREATE
        Task<RepairPaymentResponseDto?> CreatePaymentAsync(
            CreateRepairPaymentDto dto,
            Guid multitenantId,
            string? createdBy = null
        );

        // READ
        Task<RepairPaymentResponseDto?> GetPaymentByIdAsync(int id, Guid multitenantId);
        Task<RepairPaymentResponseDto?> GetPaymentByRepairIdAsync(Guid repairId, Guid multitenantId);
        Task<IEnumerable<RepairPaymentResponseDto>> GetAllPaymentsAsync(Guid multitenantId);
        Task<IEnumerable<RepairPaymentSummaryDto>> GetPaymentsSummaryAsync(Guid multitenantId, int page = 1, int pageSize = 50);

        // UPDATE
        Task<RepairPaymentResponseDto?> UpdatePaymentAsync(
            int id,
            UpdateRepairPaymentDto dto,
            Guid multitenantId,
            string? updatedBy = null
        );

        // DELETE (soft delete)
        Task<bool> DeletePaymentAsync(int id, Guid multitenantId, string? deletedBy = null);

        // RESTORE
        Task<bool> RestorePaymentAsync(int id, Guid multitenantId);

        // STATISTICS
        Task<decimal> GetTotalRevenueAsync(Guid multitenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Dictionary<string, decimal>> GetPaymentStatisticsAsync(Guid multitenantId, DateTime? startDate = null, DateTime? endDate = null);
    }
}