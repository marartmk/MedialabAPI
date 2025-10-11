using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediaLabAPI.DTOs;
using MediaLabAPI.DTOs.Repair;
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
        private readonly IQuickRepairNoteService _quickRepairNoteService;
        private readonly ILogger<RepairController> _logger;

        public RepairController(
            IRepairService repairService,
            IQuickRepairNoteService quickRepairNoteService,
            ILogger<RepairController> logger)
        {
            _repairService = repairService;
            _quickRepairNoteService = quickRepairNoteService;
            _logger = logger;
        }

        #region Standard Repairs (Existing Methods)

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
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult> UpdateRepairStatus(Guid id, [FromBody] UpdateRepairStatusDto request)
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
        /// Ottiene i dati delle riparazioni per una lista light
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

        #endregion

        #region Quick Repair Notes (NEW)

        /// <summary>
        /// Crea una nuova nota di riparazione veloce
        /// </summary>
        [HttpPost("quick-note")]
        public async Task<ActionResult<QuickRepairNoteResponseDto>> CreateQuickNote([FromBody] CreateQuickRepairNoteDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _quickRepairNoteService.CreateQuickNoteAsync(request);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating quick note");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quick note");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una nota veloce per ID intero
        /// </summary>
        [HttpGet("quick-note/{id:int}")]
        public async Task<ActionResult<QuickRepairNoteDetailDto>> GetQuickNoteById(int id)
        {
            try
            {
                var note = await _quickRepairNoteService.GetQuickNoteByIdAsync(id);

                if (note == null)
                    return NotFound(new { message = "Nota non trovata" });

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quick note {NoteId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una nota veloce per NoteId (Guid)
        /// </summary>
        [HttpGet("quick-note/guid/{noteId:guid}")]
        public async Task<ActionResult<QuickRepairNoteDetailDto>> GetQuickNoteByNoteId(Guid noteId)
        {
            try
            {
                var note = await _quickRepairNoteService.GetQuickNoteByNoteIdAsync(noteId);

                if (note == null)
                    return NotFound(new { message = "Nota non trovata" });

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting quick note {NoteId}", noteId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Cerca note veloci con filtri
        /// </summary>
        [HttpPost("quick-note/search")]
        public async Task<ActionResult<IEnumerable<QuickRepairNoteDetailDto>>> SearchQuickNotes([FromBody] QuickRepairNoteSearchDto searchDto)
        {
            try
            {
                var notes = await _quickRepairNoteService.SearchQuickNotesAsync(searchDto);
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching quick notes");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna una nota veloce
        /// </summary>
        [HttpPut("quick-note/{noteId:guid}")]
        public async Task<ActionResult> UpdateQuickNote(Guid noteId, [FromBody] UpdateQuickRepairNoteDto request)
        {
            try
            {
                await _quickRepairNoteService.UpdateQuickNoteAsync(noteId, request);
                return Ok(new { message = "Nota aggiornata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quick note {NoteId}", noteId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Elimina una nota veloce (soft delete)
        /// </summary>
        [HttpDelete("quick-note/{noteId:guid}")]
        public async Task<ActionResult> DeleteQuickNote(Guid noteId)
        {
            try
            {
                await _quickRepairNoteService.DeleteQuickNoteAsync(noteId);
                return Ok(new { message = "Nota eliminata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quick note {NoteId}", noteId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion
    }

    // DTO per aggiornamento stato (già esistente)
    public class UpdateRepairStatusDto
    {
        public string StatusCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}