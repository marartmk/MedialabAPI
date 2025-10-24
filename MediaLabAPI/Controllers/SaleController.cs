using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediaLabAPI.DTOs.Sale;
using MediaLabAPI.Services;

namespace MediaLabAPI.Controllers
{
    /// <summary>
    /// Controller per la gestione delle vendite di apparati e accessori
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly ILogger<SaleController> _logger;

        public SaleController(
            ISaleService saleService,
            ILogger<SaleController> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        #region CRUD Operations

        /// <summary>
        /// Crea una nuova vendita
        /// </summary>
        /// <param name="request">Dati della vendita da creare</param>
        /// <returns>Dettagli della vendita creata</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreateSaleResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CreateSaleResponseDto>> CreateSale([FromBody] CreateSaleRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _saleService.CreateSaleAsync(request);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating sale");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una vendita per ID intero
        /// </summary>
        /// <param name="id">ID numerico della vendita</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SaleDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SaleDetailDto>> GetSaleById(int id)
        {
            try
            {
                var sale = await _saleService.GetSaleByIdAsync(id);

                if (sale == null)
                    return NotFound(new { message = "Vendita non trovata" });

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale {SaleId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una vendita per SaleId (Guid)
        /// </summary>
        /// <param name="saleId">GUID univoco della vendita</param>
        [HttpGet("guid/{saleId:guid}")]
        [ProducesResponseType(typeof(SaleDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SaleDetailDto>> GetSaleBySaleId(Guid saleId)
        {
            try
            {
                var sale = await _saleService.GetSaleBySaleIdAsync(saleId);

                if (sale == null)
                    return NotFound(new { message = "Vendita non trovata" });

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una vendita per codice vendita
        /// </summary>
        /// <param name="saleCode">Codice vendita (es: SALE-2024-00001)</param>
        [HttpGet("code/{saleCode}")]
        [ProducesResponseType(typeof(SaleDetailDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<SaleDetailDto>> GetSaleBySaleCode(string saleCode)
        {
            try
            {
                var sale = await _saleService.GetSaleBySaleCodeAsync(saleCode);

                if (sale == null)
                    return NotFound(new { message = "Vendita non trovata" });

                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sale {SaleCode}", saleCode);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Cerca vendite con filtri avanzati
        /// </summary>
        /// <param name="searchRequest">Criteri di ricerca</param>
        [HttpPost("search")]
        [ProducesResponseType(typeof(IEnumerable<SaleDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SaleDetailDto>>> SearchSales([FromBody] SaleSearchRequestDto searchRequest)
        {
            try
            {
                var sales = await _saleService.SearchSalesAsync(searchRequest);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sales");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene tutte le vendite per un tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SaleDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SaleDetailDto>>> GetSales(
            [FromQuery] Guid? multitenantId,
            [FromQuery] string? status,
            [FromQuery] string? paymentStatus)
        {
            try
            {
                var sales = await _saleService.GetSalesAsync(multitenantId, status, paymentStatus);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le vendite per cliente
        /// </summary>
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SaleDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SaleDetailDto>>> GetSalesByCustomer(Guid customerId)
        {
            try
            {
                var sales = await _saleService.GetSalesByCustomerAsync(customerId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le vendite per dispositivo
        /// </summary>
        [HttpGet("device/{deviceId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<SaleDetailDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SaleDetailDto>>> GetSalesByDevice(Guid deviceId)
        {
            try
            {
                var sales = await _saleService.GetSalesByDeviceAsync(deviceId);
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales for device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna una vendita
        /// </summary>
        [HttpPut("{saleId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateSale(Guid saleId, [FromBody] UpdateSaleRequestDto request)
        {
            try
            {
                await _saleService.UpdateSaleAsync(saleId, request);
                return Ok(new { message = "Vendita aggiornata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vendita non trovata" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna lo stato di una vendita
        /// </summary>
        [HttpPut("{saleId:guid}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UpdateSaleStatus(Guid saleId, [FromBody] UpdateSaleStatusDto request)
        {
            try
            {
                await _saleService.UpdateSaleStatusAsync(saleId, request);
                return Ok(new { message = "Stato aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vendita non trovata" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale status {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Elimina una vendita (soft delete)
        /// </summary>
        [HttpDelete("{saleId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteSale(Guid saleId)
        {
            try
            {
                await _saleService.DeleteSaleAsync(saleId);
                return Ok(new { message = "Vendita eliminata con successo" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vendita non trovata" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Payment Management

        /// <summary>
        /// Aggiunge un pagamento a una vendita
        /// </summary>
        [HttpPost("{saleId:guid}/payment")]
        [ProducesResponseType(typeof(AddSalePaymentResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AddSalePaymentResponseDto>> AddPayment(
            Guid saleId,
            [FromBody] SalePaymentDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _saleService.AddPaymentAsync(saleId, paymentDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error adding payment to sale {SaleId}", saleId);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vendita non trovata" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding payment to sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene tutti i pagamenti di una vendita
        /// </summary>
        [HttpGet("{saleId:guid}/payments")]
        [ProducesResponseType(typeof(IEnumerable<SalePaymentDetailDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<SalePaymentDetailDto>>> GetSalePayments(Guid saleId)
        {
            try
            {
                var payments = await _saleService.GetSalePaymentsAsync(saleId);
                return Ok(payments);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vendita non trovata" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments for sale {SaleId}", saleId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Elimina un pagamento (soft delete)
        /// </summary>
        [HttpDelete("payment/{paymentId:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeletePayment(Guid paymentId)
        {
            try
            {
                await _saleService.DeletePaymentAsync(paymentId);
                return Ok(new { message = "Pagamento eliminato con successo" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Pagamento non trovato" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment {PaymentId}", paymentId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Ottiene statistiche vendite per un periodo
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> GetSalesStatistics(
            [FromQuery] Guid? multitenantId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var stats = await _saleService.GetSalesStatisticsAsync(multitenantId, fromDate, toDate);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales statistics");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion
    }
}