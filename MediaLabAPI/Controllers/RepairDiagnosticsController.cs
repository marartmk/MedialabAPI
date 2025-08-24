using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediaLabAPI.DTOs;
using MediaLabAPI.Services;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/repair/{repairId:guid}")]
    [Authorize]
    public class RepairDiagnosticsController : ControllerBase
    {
        private readonly IRepairService _repairService;
        private readonly ILogger<RepairDiagnosticsController> _logger;

        public RepairDiagnosticsController(IRepairService repairService, ILogger<RepairDiagnosticsController> logger)
        {
            _repairService = repairService;
            _logger = logger;
        }

        [HttpGet("incoming-test")]
        public async Task<ActionResult<IncomingTestDto>> GetIncoming(Guid repairId)
        {
            try
            {
                var dto = await _repairService.GetIncomingAsync(repairId);
                return dto is null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incoming test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        [HttpPut("incoming-test")]
        public async Task<IActionResult> UpsertIncoming(Guid repairId, [FromBody] IncomingTestDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _repairService.UpsertIncomingAsync(repairId, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error upserting incoming test for {RepairId}", repairId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting incoming test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        [HttpDelete("incoming-test")]
        public async Task<IActionResult> DeleteIncoming(Guid repairId)
        {
            try
            {
                await _repairService.DeleteIncomingAsync(repairId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incoming test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        [HttpGet("exit-test")]
        public async Task<ActionResult<ExitTestDto>> GetExit(Guid repairId)
        {
            try
            {
                var dto = await _repairService.GetExitAsync(repairId);
                return dto is null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exit test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        [HttpPut("exit-test")]
        public async Task<IActionResult> UpsertExit(Guid repairId, [FromBody] ExitTestDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _repairService.UpsertExitAsync(repairId, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error upserting exit test for {RepairId}", repairId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting exit test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        [HttpDelete("exit-test")]
        public async Task<IActionResult> DeleteExit(Guid repairId)
        {
            try
            {
                await _repairService.DeleteExitAsync(repairId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exit test for {RepairId}", repairId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }
    }

}

