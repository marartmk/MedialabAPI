using MediaLabAPI.Data;
using MediaLabAPI.DTOs;
using MediaLabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.Services
{
    public class RepairPaymentService : IRepairPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RepairPaymentService> _logger;

        public RepairPaymentService(AppDbContext context, ILogger<RepairPaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // CREATE
        public async Task<RepairPaymentResponseDto?> CreatePaymentAsync(
            CreateRepairPaymentDto dto,
            Guid multitenantId,
            string? createdBy = null)
        {
            try
            {
                // Verifica che la riparazione esista
                var repair = await _context.DeviceRepairs
                    .FirstOrDefaultAsync(r => r.RepairId == dto.RepairId && r.MultitenantId == multitenantId && !r.IsDeleted);

                if (repair == null)
                {
                    _logger.LogWarning("Riparazione {RepairId} non trovata per multitenant {MultitenantId}", dto.RepairId, multitenantId);
                    return null;
                }

                // Verifica che non esista già un pagamento per questa riparazione
                var existingPayment = await _context.RepairPayments
                    .FirstOrDefaultAsync(p => p.RepairId == dto.RepairId && p.MultitenantId == multitenantId && !p.IsDeleted);

                if (existingPayment != null)
                {
                    _logger.LogWarning("Pagamento già esistente per riparazione {RepairId}", dto.RepairId);
                    return null;
                }

                var payment = new RepairPayment
                {
                    RepairId = dto.RepairId,
                    PartsAmount = dto.PartsAmount,
                    LaborAmount = dto.LaborAmount,
                    CompanyId = repair.CompanyId,
                    MultitenantId = multitenantId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                    Notes = dto.Notes,
                    IsDeleted = false
                };

                _context.RepairPayments.Add(payment);
                await _context.SaveChangesAsync();

                // Ricarica per ottenere i campi calcolati
                await _context.Entry(payment).ReloadAsync();

                return await MapToResponseDto(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del pagamento per riparazione {RepairId}", dto.RepairId);
                throw;
            }
        }

        // READ - By ID
        public async Task<RepairPaymentResponseDto?> GetPaymentByIdAsync(int id, Guid multitenantId)
        {
            var payment = await _context.RepairPayments
                .Include(p => p.DeviceRepair)
                .FirstOrDefaultAsync(p => p.Id == id && p.MultitenantId == multitenantId && !p.IsDeleted);

            return payment != null ? await MapToResponseDto(payment) : null;
        }

        // READ - By RepairId
        public async Task<RepairPaymentResponseDto?> GetPaymentByRepairIdAsync(Guid repairId, Guid multitenantId)
        {
            var payment = await _context.RepairPayments
                .Include(p => p.DeviceRepair)
                .FirstOrDefaultAsync(p => p.RepairId == repairId && p.MultitenantId == multitenantId && !p.IsDeleted);

            return payment != null ? await MapToResponseDto(payment) : null;
        }

        // READ - All
        public async Task<IEnumerable<RepairPaymentResponseDto>> GetAllPaymentsAsync(Guid multitenantId)
        {
            var payments = await _context.RepairPayments
                .Include(p => p.DeviceRepair)
                .Where(p => p.MultitenantId == multitenantId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = new List<RepairPaymentResponseDto>();
            foreach (var payment in payments)
            {
                var dto = await MapToResponseDto(payment);
                if (dto != null)
                    result.Add(dto);
            }

            return result;
        }

        // READ - Summary (con paginazione)
        public async Task<IEnumerable<RepairPaymentSummaryDto>> GetPaymentsSummaryAsync(
            Guid multitenantId,
            int page = 1,
            int pageSize = 50)
        {
            return await _context.RepairPayments
                .Include(p => p.DeviceRepair)
                .Where(p => p.MultitenantId == multitenantId && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new RepairPaymentSummaryDto
                {
                    Id = p.Id,
                    RepairId = p.RepairId,
                    RepairCode = p.DeviceRepair != null ? p.DeviceRepair.RepairCode : null,
                    TotalAmount = p.TotalAmount ?? 0,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = p.CreatedBy
                })
                .ToListAsync();
        }

        // UPDATE
        public async Task<RepairPaymentResponseDto?> UpdatePaymentAsync(
            int id,
            UpdateRepairPaymentDto dto,
            Guid multitenantId,
            string? updatedBy = null)
        {
            try
            {
                var payment = await _context.RepairPayments
                    .Include(p => p.DeviceRepair)
                    .FirstOrDefaultAsync(p => p.Id == id && p.MultitenantId == multitenantId && !p.IsDeleted);

                if (payment == null)
                {
                    _logger.LogWarning("Pagamento {Id} non trovato per multitenant {MultitenantId}", id, multitenantId);
                    return null;
                }

                payment.PartsAmount = dto.PartsAmount;
                payment.LaborAmount = dto.LaborAmount;
                payment.Notes = dto.Notes;

                await _context.SaveChangesAsync();

                // Ricarica per ottenere i campi calcolati aggiornati
                await _context.Entry(payment).ReloadAsync();

                return await MapToResponseDto(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del pagamento {Id}", id);
                throw;
            }
        }

        // DELETE (soft delete)
        public async Task<bool> DeletePaymentAsync(int id, Guid multitenantId, string? deletedBy = null)
        {
            try
            {
                var payment = await _context.RepairPayments
                    .FirstOrDefaultAsync(p => p.Id == id && p.MultitenantId == multitenantId && !p.IsDeleted);

                if (payment == null)
                    return false;

                payment.IsDeleted = true;
                payment.DeletedAt = DateTime.Now;
                payment.DeletedBy = deletedBy;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del pagamento {Id}", id);
                throw;
            }
        }

        // RESTORE
        public async Task<bool> RestorePaymentAsync(int id, Guid multitenantId)
        {
            try
            {
                var payment = await _context.RepairPayments
                    .FirstOrDefaultAsync(p => p.Id == id && p.MultitenantId == multitenantId && p.IsDeleted);

                if (payment == null)
                    return false;

                payment.IsDeleted = false;
                payment.DeletedAt = null;
                payment.DeletedBy = null;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il ripristino del pagamento {Id}", id);
                throw;
            }
        }

        // STATISTICS - Total Revenue
        public async Task<decimal> GetTotalRevenueAsync(
            Guid multitenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.RepairPayments
                .Where(p => p.MultitenantId == multitenantId && !p.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            return await query.SumAsync(p => p.TotalAmount ?? 0);
        }

        // STATISTICS - Detailed
        public async Task<Dictionary<string, decimal>> GetPaymentStatisticsAsync(
            Guid multitenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.RepairPayments
                .Where(p => p.MultitenantId == multitenantId && !p.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            var payments = await query.ToListAsync();

            return new Dictionary<string, decimal>
            {
                { "TotalRevenue", payments.Sum(p => p.TotalAmount ?? 0) },
                { "TotalParts", payments.Sum(p => p.PartsAmount) },
                { "TotalLabor", payments.Sum(p => p.LaborAmount) },
                { "TotalVAT", payments.Sum(p => p.VatAmount ?? 0) },
                { "AveragePayment", payments.Any() ? payments.Average(p => p.TotalAmount ?? 0) : 0 },
                { "PaymentCount", payments.Count }
            };
        }

        // HELPER - Mapping
        private async Task<RepairPaymentResponseDto?> MapToResponseDto(RepairPayment payment)
        {
            if (payment.DeviceRepair == null)
            {
                await _context.Entry(payment).Reference(p => p.DeviceRepair).LoadAsync();
            }

            return new RepairPaymentResponseDto
            {
                Id = payment.Id,
                RepairId = payment.RepairId,
                RepairCode = payment.DeviceRepair?.RepairCode,
                PartsAmount = payment.PartsAmount,
                LaborAmount = payment.LaborAmount,
                VatAmount = payment.VatAmount ?? 0,
                TotalAmount = payment.TotalAmount ?? 0,
                CompanyId = payment.CompanyId,
                MultitenantId = payment.MultitenantId,
                CreatedAt = payment.CreatedAt,
                CreatedBy = payment.CreatedBy,
                Notes = payment.Notes,
                IsDeleted = payment.IsDeleted
            };
        }
    }
}