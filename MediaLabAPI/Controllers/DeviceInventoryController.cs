using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediaLabAPI.DTOs;
using MediaLabAPI.DTOs.DeviceInventory;
using MediaLabAPI.Services;
using MediaLabAPI.Models;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeviceInventoryController : ControllerBase
    {
        private readonly IDeviceInventoryService _deviceInventoryService;
        private readonly ILogger<DeviceInventoryController> _logger;

        public DeviceInventoryController(
            IDeviceInventoryService deviceInventoryService,
            ILogger<DeviceInventoryController> logger)
        {
            _deviceInventoryService = deviceInventoryService;
            _logger = logger;
        }

        /// <summary>
        /// Ottiene tutti gli apparati con filtri e paginazione
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<DeviceInventoryPagedResponseDto>> SearchDevices([FromBody] DeviceInventorySearchDto searchDto)
        {
            try
            {
                var result = await _deviceInventoryService.SearchDevicesAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca degli apparati");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un singolo apparato per ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceInventoryDto>> GetDeviceById(int id)
        {
            try
            {
                var device = await _deviceInventoryService.GetDeviceByIdAsync(id);
                if (device == null)
                {
                    return NotFound(new { message = "Apparato non trovato" });
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un apparato per DeviceId (GUID)
        /// </summary>
        [HttpGet("device/{deviceId:guid}")]
        public async Task<ActionResult<DeviceInventoryDto>> GetDeviceByGuid(Guid deviceId)
        {
            try
            {
                var device = await _deviceInventoryService.GetDeviceByGuidAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = "Apparato non trovato" });
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'apparato {DeviceGuid}", deviceId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un apparato per codice
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<DeviceInventoryDto>> GetDeviceByCode(string code)
        {
            try
            {
                var device = await _deviceInventoryService.GetDeviceByCodeAsync(code);
                if (device == null)
                {
                    return NotFound(new { message = "Apparato non trovato" });
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'apparato con codice {Code}", code);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un apparato per IMEI
        /// </summary>
        [HttpGet("imei/{imei}")]
        public async Task<ActionResult<DeviceInventoryDto>> GetDeviceByImei(string imei)
        {
            try
            {
                var device = await _deviceInventoryService.GetDeviceByImeiAsync(imei);
                if (device == null)
                {
                    return NotFound(new { message = "Apparato non trovato" });
                }
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'apparato con IMEI {IMEI}", imei);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Crea un nuovo apparato
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateDeviceInventoryResponseDto>> CreateDevice([FromBody] CreateDeviceInventoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _deviceInventoryService.CreateDeviceAsync(createDto);
                return CreatedAtAction(nameof(GetDeviceById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Errore di validazione durante la creazione dell'apparato");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione dell'apparato");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna un apparato esistente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDevice(int id, [FromBody] UpdateDeviceInventoryDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _deviceInventoryService.UpdateDeviceAsync(id, updateDto);
                return Ok(new { message = "Apparato aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento dell'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Elimina un apparato (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDevice(int id)
        {
            try
            {
                await _deviceInventoryService.DeleteDeviceAsync(id);
                return Ok(new { message = "Apparato eliminato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le statistiche del magazzino apparati
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<DeviceInventoryStatsDto>> GetStats([FromQuery] Guid? multitenantId)
        {
            try
            {
                var stats = await _deviceInventoryService.GetStatsAsync(multitenantId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle statistiche");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene gli apparati disponibili per il prestito di cortesia
        /// </summary>
        [HttpGet("courtesy-available")]
        public async Task<ActionResult<List<DeviceInventoryDto>>> GetCourtesyAvailableDevices([FromQuery] Guid? multitenantId)
        {
            try
            {
                var devices = await _deviceInventoryService.GetCourtesyAvailableDevicesAsync(multitenantId);
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero degli apparati di cortesia disponibili");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Cambia lo stato di un apparato (es: da available a loaned)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> ChangeDeviceStatus(int id, [FromBody] ChangeDeviceStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _deviceInventoryService.ChangeDeviceStatusAsync(id, statusDto);
                return Ok(new { message = "Stato apparato aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato dell'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Registra un prestito di cortesia
        /// </summary>
        [HttpPost("{id}/loan")]
        public async Task<ActionResult> LoanDevice(int id, [FromBody] LoanDeviceDto loanDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _deviceInventoryService.LoanDeviceAsync(id, loanDto);
                return Ok(new { message = "Prestito registrato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione del prestito per l'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Registra la restituzione di un apparato in prestito
        /// </summary>
        [HttpPost("{id}/return")]
        public async Task<ActionResult> ReturnDevice(int id, [FromBody] ReturnDeviceDto returnDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _deviceInventoryService.ReturnDeviceAsync(id, returnDto);
                return Ok(new { message = "Restituzione registrata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione della restituzione per l'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene lo storico movimenti di un apparato
        /// </summary>
        [HttpGet("{id}/movements")]
        public async Task<ActionResult<List<DeviceInventoryMovementDto>>> GetDeviceMovements(int id)
        {
            try
            {
                var movements = await _deviceInventoryService.GetDeviceMovementsAsync(id);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dello storico movimenti per l'apparato {DeviceId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        // ==========================================
        // GESTIONE FORNITORI
        // ==========================================

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        [HttpGet("suppliers")]
        public async Task<ActionResult<List<DeviceInventorySupplierDto>>> GetSuppliers([FromQuery] Guid? multitenantId)
        {
            try
            {
                var suppliers = await _deviceInventoryService.GetSuppliersAsync(multitenantId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei fornitori");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Crea un nuovo fornitore
        /// </summary>
        [HttpPost("suppliers")]
        public async Task<ActionResult<DeviceInventorySupplierDto>> CreateSupplier([FromBody] CreateDeviceInventorySupplierDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _deviceInventoryService.CreateSupplierAsync(createDto);
                return CreatedAtAction(nameof(GetSuppliers), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del fornitore");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Esporta i dati degli apparati in CSV
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<ActionResult> ExportToCsv([FromQuery] Guid? multitenantId)
        {
            try
            {
                var csvData = await _deviceInventoryService.ExportToCsvAsync(multitenantId);
                var fileName = $"apparati_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'esportazione dei dati in CSV");
                return StatusCode(500, new { message = "Errore nell'esportazione dei dati" });
            }
        }
    }
}