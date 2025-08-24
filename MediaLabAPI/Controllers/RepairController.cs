using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediaLabAPI.DTOs;
using MediaLabAPI.Services;
using MediaLabAPI.Models;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RepairController : ControllerBase
    {
        private readonly IRepairService _repairService;
        private readonly ILogger<RepairController> _logger;

        public RepairController(IRepairService repairService, ILogger<RepairController> logger)
        {
            _repairService = repairService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nuova scheda di riparazione
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateRepairResponseDto>> CreateRepair([FromBody] CreateRepairRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _repairService.CreateRepairAsync(request);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating repair");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating repair");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una riparazione per ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairDetailDto>> GetRepair(int id)
        {
            try
            {
                var dto = await _repairService.GetRepairDetailByIdAsync(id);
                if (dto == null) return NotFound(new { message = "Riparazione non trovata" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting repair {RepairId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene tutte le riparazioni per un tenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceRepair>>> GetRepairs([FromQuery] Guid? multitenantId, [FromQuery] string? status)
        {
            try
            {
                var repairs = await _repairService.GetRepairsAsync(multitenantId, status);
                return Ok(repairs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting repairs");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna lo stato di una riparazione
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateRepairStatus(int id, [FromBody] UpdateRepairStatusDto request)
        {
            try
            {
                await _repairService.UpdateRepairStatusAsync(id, request.StatusCode, request.Status, request.Notes);
                return Ok(new { message = "Stato aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating repair status {RepairId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le riparazioni per cliente
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<DeviceRepair>>> GetRepairsByCustomer(Guid customerId)
        {
            try
            {
                var repairs = await _repairService.GetRepairsByCustomerAsync(customerId);
                return Ok(repairs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting repairs for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le riparazioni per dispositivo
        /// </summary>
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<DeviceRepair>>> GetRepairsByDevice(int deviceId)
        {
            try
            {
                var repairs = await _repairService.GetRepairsByDeviceAsync(deviceId);
                return Ok(repairs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting repairs for device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna i dati della riparazione usando RepairId (Guid)
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRepair(Guid id, [FromBody] UpdateRepairRequestDto request)
        {
            try
            {
                await _repairService.UpdateRepairAsync(id, request);
                return Ok(new { message = "Scheda riparazione aggiornata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante aggiornamento repair {RepairId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene i dati delle riparazioni per una lista liht
        /// </summary>
        [HttpPost("search/light")]
        public async Task<ActionResult<IEnumerable<RepairDetailDto>>> SearchRepairsLight([FromBody] RepairSearchRequestDto searchRequest)
        {
            try
            {
                var repairs = await _repairService.GetRepairsLightAsync(searchRequest);
                return Ok(repairs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching light repairs");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }
    }

    // DTO per aggiornamento stato
    public class UpdateRepairStatusDto
    {
        public string StatusCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}