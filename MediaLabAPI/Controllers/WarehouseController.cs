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
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService warehouseService, ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        /// <summary>
        /// Ottiene tutti gli articoli del magazzino con filtri e paginazione
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<WarehouseItemsPagedResponseDto>> GetWarehouseItems([FromBody] WarehouseSearchDto searchDto)
        {
            try
            {
                var result = await _warehouseService.GetWarehouseItemsAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse items");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un singolo articolo per ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseItemDetailDto>> GetWarehouseItem(int id)
        {
            try
            {
                var item = await _warehouseService.GetWarehouseItemByIdAsync(id);
                if (item == null)
                {
                    return NotFound(new { message = "Articolo non trovato" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse item {ItemId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un articolo per ItemId (GUID)
        /// </summary>
        [HttpGet("item/{itemId:guid}")]
        public async Task<ActionResult<WarehouseItemDetailDto>> GetWarehouseItemByGuid(Guid itemId)
        {
            try
            {
                var item = await _warehouseService.GetWarehouseItemByGuidAsync(itemId);
                if (item == null)
                {
                    return NotFound(new { message = "Articolo non trovato" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse item {ItemGuid}", itemId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene un articolo per codice
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<WarehouseItemDetailDto>> GetWarehouseItemByCode(string code)
        {
            try
            {
                var item = await _warehouseService.GetWarehouseItemByCodeAsync(code);
                if (item == null)
                {
                    return NotFound(new { message = "Articolo non trovato" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse item by code {Code}", code);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Crea un nuovo articolo di magazzino
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateWarehouseItemResponseDto>> CreateWarehouseItem([FromBody] CreateWarehouseItemDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _warehouseService.CreateWarehouseItemAsync(createDto);
                return CreatedAtAction(nameof(GetWarehouseItem), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating warehouse item");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse item");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna un articolo esistente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWarehouseItem(int id, [FromBody] UpdateWarehouseItemDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _warehouseService.UpdateWarehouseItemAsync(id, updateDto);
                return Ok(new { message = "Articolo aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating warehouse item {ItemId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Elimina un articolo (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWarehouseItem(int id)
        {
            try
            {
                await _warehouseService.DeleteWarehouseItemAsync(id);
                return Ok(new { message = "Articolo eliminato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting warehouse item {ItemId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna la quantità di un articolo
        /// </summary>
        [HttpPut("{id}/quantity")]
        public async Task<ActionResult> UpdateQuantity(int id, [FromBody] UpdateQuantityDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _warehouseService.UpdateQuantityAsync(id, updateDto);
                return Ok(new { message = "Quantità aggiornata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity for item {ItemId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le statistiche del magazzino
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<WarehouseStatsDto>> GetWarehouseStats([FromQuery] Guid? multitenantId)
        {
            try
            {
                var stats = await _warehouseService.GetWarehouseStatsAsync(multitenantId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse statistics");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene gli articoli con scorte basse
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<List<WarehouseItemDetailDto>>> GetLowStockItems([FromQuery] Guid? multitenantId)
        {
            try
            {
                var items = await _warehouseService.GetLowStockItemsAsync(multitenantId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock items");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene la lista semplificata degli articoli (per dropdown)
        /// </summary>
        [HttpGet("light")]
        public async Task<ActionResult<List<WarehouseItemLightDto>>> GetWarehouseItemsLight([FromQuery] Guid? multitenantId, [FromQuery] string? category)
        {
            try
            {
                var items = await _warehouseService.GetWarehouseItemsLightAsync(multitenantId, category);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving light warehouse items");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ricerca rapida per codice o nome
        /// </summary>
        [HttpGet("search-quick")]
        public async Task<ActionResult<List<WarehouseItemLightDto>>> QuickSearch([FromQuery] string query, [FromQuery] Guid? multitenantId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { message = "Query di ricerca richiesta" });
                }

                var items = await _warehouseService.QuickSearchAsync(query, multitenantId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing quick search with query {Query}", query);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Registra un movimento di magazzino
        /// </summary>
        [HttpPost("movement")]
        public async Task<ActionResult> RegisterMovement([FromBody] WarehouseMovementDto movementDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _warehouseService.RegisterMovementAsync(movementDto);
                return Ok(new { message = "Movimento registrato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering warehouse movement");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        // CATEGORIE

        /// <summary>
        /// Ottiene tutte le categorie
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetCategories([FromQuery] Guid? multitenantId)
        {
            try
            {
                var categories = await _warehouseService.GetCategoriesAsync(multitenantId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse categories");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Crea una nuova categoria
        /// </summary>
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryInfoDto>> CreateCategory([FromBody] CreateWarehouseCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _warehouseService.CreateCategoryAsync(createDto);
                return CreatedAtAction(nameof(GetCategories), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse category");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        // FORNITORI

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        [HttpGet("suppliers")]
        public async Task<ActionResult<List<WarehouseSupplier>>> GetSuppliers([FromQuery] Guid? multitenantId)
        {
            try
            {
                var suppliers = await _warehouseService.GetSuppliersAsync(multitenantId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving warehouse suppliers");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Crea un nuovo fornitore
        /// </summary>
        [HttpPost("suppliers")]
        public async Task<ActionResult<WarehouseSupplier>> CreateSupplier([FromBody] CreateWarehouseSupplierDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _warehouseService.CreateSupplierAsync(createDto);
                return CreatedAtAction(nameof(GetSuppliers), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating warehouse supplier");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Esporta i dati del magazzino in CSV
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<ActionResult> ExportToCsv([FromQuery] Guid? multitenantId)
        {
            try
            {
                var csvData = await _warehouseService.ExportToCsvAsync(multitenantId);
                var fileName = $"magazzino_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting warehouse data to CSV");
                return StatusCode(500, new { message = "Errore nell'esportazione dei dati" });
            }
        }
    }
}