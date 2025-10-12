using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaLabAPI.DTOs.RepairParts;
using MediaLabAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Richiede autenticazione
    public class RepairPartsController : ControllerBase
    {
        private readonly IRepairPartsService _repairPartsService;

        public RepairPartsController(IRepairPartsService repairPartsService)
        {
            _repairPartsService = repairPartsService;
        }

        /// <summary>
        /// Ottiene tutti i ricambi di una riparazione
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        [HttpGet("{repairId}")]
        [ProducesResponseType(typeof(List<RepairPartDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<RepairPartDto>>> GetRepairParts(Guid repairId)
        {
            try
            {
                var parts = await _repairPartsService.GetRepairPartsAsync(repairId);
                return Ok(parts);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Aggiunge un ricambio a una riparazione
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        /// <param name="dto">Dati del ricambio da aggiungere</param>
        [HttpPost("{repairId}")]
        [ProducesResponseType(typeof(RepairPartDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RepairPartDto>> AddRepairPart(
            Guid repairId,
            [FromBody] CreateRepairPartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdBy = User?.Identity?.Name; // Ottiene username dall'autenticazione
                var result = await _repairPartsService.AddRepairPartAsync(repairId, dto, createdBy);

                return CreatedAtAction(
                    nameof(GetRepairParts),
                    new { repairId = result.RepairId },
                    result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Aggiunge più ricambi in batch
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        /// <param name="dto">Lista di ricambi da aggiungere</param>
        [HttpPost("{repairId}/batch")]
        [ProducesResponseType(typeof(List<RepairPartDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<List<RepairPartDto>>> AddRepairPartsBatch(
            Guid repairId,
            [FromBody] CreateRepairPartBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdBy = User?.Identity?.Name;
                var result = await _repairPartsService.AddRepairPartsBatchAsync(repairId, dto.Parts, createdBy);
                return CreatedAtAction(nameof(GetRepairParts), new { repairId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Aggiorna un ricambio esistente
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        /// <param name="partId">ID del ricambio</param>
        /// <param name="dto">Dati aggiornati</param>
        [HttpPut("{repairId}/parts/{partId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRepairPart(
            Guid repairId,
            int partId,
            [FromBody] UpdateRepairPartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _repairPartsService.UpdateRepairPartAsync(repairId, partId, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Rimuove un ricambio da una riparazione
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        /// <param name="partId">ID del ricambio</param>
        [HttpDelete("{repairId}/parts/{partId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRepairPart(Guid repairId, int partId)
        {
            try
            {
                await _repairPartsService.DeleteRepairPartAsync(repairId, partId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Ottiene il totale dei ricambi di una riparazione
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        [HttpGet("{repairId}/total")]
        [ProducesResponseType(typeof(RepairPartsTotalDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<RepairPartsTotalDto>> GetRepairPartsTotal(Guid repairId)
        {
            try
            {
                var total = await _repairPartsService.GetRepairPartsTotalAsync(repairId);
                return Ok(total);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }

        /// <summary>
        /// Scarica i ricambi dal magazzino (consume stock)
        /// </summary>
        /// <param name="repairId">GUID della riparazione</param>
        [HttpPost("{repairId}/consume")]
        [ProducesResponseType(typeof(ConsumeStockResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ConsumeStockResponseDto>> ConsumeStock(Guid repairId)
        {
            try
            {
                var processedBy = User?.Identity?.Name;
                var result = await _repairPartsService.ConsumeStockAsync(repairId, processedBy);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Errore interno del server", details = ex.Message });
            }
        }
    }
}