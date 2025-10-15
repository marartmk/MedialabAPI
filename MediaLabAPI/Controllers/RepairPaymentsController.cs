using MediaLabAPI.DTOs;
using MediaLabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediaLabAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RepairPaymentsController : ControllerBase
    {
        private readonly IRepairPaymentService _paymentService;
        private readonly ILogger<RepairPaymentsController> _logger;
        private readonly AppDbContext _db;

        public RepairPaymentsController(
               IRepairPaymentService paymentService,
               ILogger<RepairPaymentsController> logger,
                AppDbContext db) // ⬅️ nuovo
        {
            _paymentService = paymentService;
            _logger = logger;
            _db = db; // ⬅️ nuovo
        }

        // GET: api/RepairPayments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepairPaymentResponseDto>>> GetAllPayments()
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var payments = await _paymentService.GetAllPaymentsAsync(multitenantId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei pagamenti");
                return StatusCode(500, "Errore interno del server");
            }
        }

        // GET: api/RepairPayments/summary?page=1&pageSize=50
        [HttpGet("summary")]
        public async Task<ActionResult<IEnumerable<RepairPaymentSummaryDto>>> GetPaymentsSummary(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var summary = await _paymentService.GetPaymentsSummaryAsync(multitenantId, page, pageSize);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del riepilogo pagamenti");
                return StatusCode(500, "Errore interno del server");
            }
        }

        // GET: api/RepairPayments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairPaymentResponseDto>> GetPaymentById(int id)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var payment = await _paymentService.GetPaymentByIdAsync(id, multitenantId);

                if (payment == null)
                    return NotFound($"Pagamento con ID {id} non trovato");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del pagamento {Id}", id);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // GET: api/RepairPayments/repair/{repairId}
        [HttpGet("repair/{repairId}")]
        public async Task<ActionResult<RepairPaymentResponseDto>> GetPaymentByRepairId(Guid repairId)
        {
            try
            {
                var multitenantId = await ResolveMultitenantIdAsync(repairId);
                var payment = await _paymentService.GetPaymentByRepairIdAsync(repairId, multitenantId);

                if (payment == null)
                    return NotFound($"Pagamento per riparazione {repairId} non trovato");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del pagamento per riparazione {RepairId}", repairId);
                return StatusCode(500, "Errore interno del server");
            }
        }


        // POST: api/RepairPayments
        [HttpPost]
        public async Task<ActionResult<RepairPaymentResponseDto>> CreatePayment(
            [FromBody] CreateRepairPaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //var multitenantId = GetMultitenantId();
                var multitenantId = dto.MultitenantId;
                var createdBy = GetUserName();

                var payment = await _paymentService.CreatePaymentAsync(dto, multitenantId, createdBy);

                if (payment == null)
                    return BadRequest("Impossibile creare il pagamento. Verificare che la riparazione esista e non abbia già un pagamento associato.");

                return CreatedAtAction(
                    nameof(GetPaymentById),
                    new { id = payment.Id },
                    payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del pagamento");
                return StatusCode(500, "Errore interno del server");
            }
        }

        // PUT: api/RepairPayments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<RepairPaymentResponseDto>> UpdatePayment(
            int id,
            [FromBody] UpdateRepairPaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var multitenantId = dto.MultitenantId;
                var updatedBy = GetUserName();

                var payment = await _paymentService.UpdatePaymentAsync(id, dto, multitenantId, updatedBy);

                if (payment == null)
                    return NotFound($"Pagamento con ID {id} non trovato");

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del pagamento {Id}", id);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // DELETE: api/RepairPayments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayment(int id)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var deletedBy = GetUserName();

                var result = await _paymentService.DeletePaymentAsync(id, multitenantId, deletedBy);

                if (!result)
                    return NotFound($"Pagamento con ID {id} non trovato");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del pagamento {Id}", id);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // POST: api/RepairPayments/5/restore
        [HttpPost("{id}/restore")]
        public async Task<ActionResult> RestorePayment(int id)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var result = await _paymentService.RestorePaymentAsync(id, multitenantId);

                if (!result)
                    return NotFound($"Pagamento eliminato con ID {id} non trovato");

                return Ok(new { message = "Pagamento ripristinato con successo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il ripristino del pagamento {Id}", id);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // GET: api/RepairPayments/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult<Dictionary<string, decimal>>> GetStatistics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var statistics = await _paymentService.GetPaymentStatisticsAsync(multitenantId, startDate, endDate);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle statistiche pagamenti");
                return StatusCode(500, "Errore interno del server");
            }
        }

        // GET: api/RepairPayments/revenue
        [HttpGet("revenue")]
        public async Task<ActionResult<decimal>> GetTotalRevenue(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var multitenantId = GetMultitenantId();
                var revenue = await _paymentService.GetTotalRevenueAsync(multitenantId, startDate, endDate);
                return Ok(new { totalRevenue = revenue });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel calcolo del fatturato totale");
                return StatusCode(500, "Errore interno del server");
            }
        }

        // HELPER METHODS
        private Guid GetMultitenantId(string repairId)
        {
            var multitenantIdClaim = User.FindFirst("MultitenantId")?.Value;
            if (string.IsNullOrEmpty(multitenantIdClaim) || !Guid.TryParse(multitenantIdClaim, out var multitenantId))
            {
                throw new UnauthorizedAccessException("MultitenantId non valido nel token");
            }
            return multitenantId;
        }

        // ========== HELPER METHODS ==========

        // 1) Risolve il MultitenantId:
        //    - prima tenta dal claim
        //    - se non c'è/è invalido e ho un RepairId, lo legge da DeviceRepairs
        private async Task<Guid> ResolveMultitenantIdAsync(Guid? repairId = null)
        {
            var claim = User.FindFirst("MultitenantId")?.Value;
            if (!string.IsNullOrWhiteSpace(claim) && Guid.TryParse(claim, out var mtFromToken))
                return mtFromToken;

            if (repairId.HasValue)
            {
                var mtFromDb = await _db.DeviceRepairs
                    .AsNoTracking()
                    .Where(r => r.RepairId == repairId.Value && (r.IsDeleted == false || r.IsDeleted == null))
                    .Select(r => (Guid?)r.MultitenantId)
                    .FirstOrDefaultAsync();

                if (mtFromDb.HasValue)
                    return mtFromDb.Value;
            }

            throw new UnauthorizedAccessException(
                "Impossibile determinare il MultitenantId (claim assente e riparazione non trovata).");
        }

        // Rimane per gli endpoint che NON hanno un RepairId (summary, revenue, ecc.)
        private Guid GetMultitenantId()
        {
            var multitenantIdClaim = User.FindFirst("MultitenantId")?.Value;
            if (string.IsNullOrEmpty(multitenantIdClaim) || !Guid.TryParse(multitenantIdClaim, out var multitenantId))
                throw new UnauthorizedAccessException("MultitenantId non valido nel token");
            return multitenantId;
        }

        private string? GetUserName() =>
            User.Identity?.Name ?? User.FindFirst("sub")?.Value;

    }
}