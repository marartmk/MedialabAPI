using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediaLabAPI.DTOs.Booking;
using MediaLabAPI.Services;

namespace MediaLabAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(
            IBookingService bookingService,
            ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        #region Create

        /// <summary>
        /// Crea una nuova prenotazione
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateBookingResponseDto>> CreateBooking(
            [FromBody] CreateBookingRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _bookingService.CreateBookingAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error creating booking");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Read - Get by ID

        /// <summary>
        /// Ottiene una prenotazione per ID intero
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookingDetailDto>> GetBooking(int id)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null)
                    return NotFound(new { message = "Prenotazione non trovata" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking {BookingId}", id);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una prenotazione per BookingId (Guid)
        /// </summary>
        [HttpGet("guid/{bookingId:guid}")]
        public async Task<ActionResult<BookingDetailDto>> GetBookingByGuid(Guid bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByGuidAsync(bookingId);
                if (booking == null)
                    return NotFound(new { message = "Prenotazione non trovata" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking {BookingId}", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene una prenotazione per codice
        /// </summary>
        [HttpGet("code/{bookingCode}")]
        public async Task<ActionResult<BookingDetailDto>> GetBookingByCode(string bookingCode)
        {
            try
            {
                var booking = await _bookingService.GetBookingByCodeAsync(bookingCode);
                if (booking == null)
                    return NotFound(new { message = "Prenotazione non trovata" });

                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking by code {BookingCode}", bookingCode);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Read - Lists and Filters

        /// <summary>
        /// Ottiene tutte le prenotazioni
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetAllBookings(
            [FromQuery] Guid? multitenantId)
        {
            try
            {
                var bookings = await _bookingService.GetAllBookingsAsync(multitenantId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Cerca prenotazioni con filtri avanzati
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<BookingListDto>>> SearchBookings(
            [FromBody] BookingSearchRequestDto searchRequest)
        {
            try
            {
                var bookings = await _bookingService.SearchBookingsAsync(searchRequest);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bookings");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le prenotazioni per un cliente specifico
        /// </summary>
        [HttpGet("customer/{customerId:guid}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByCustomer(
            Guid customerId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByCustomerAsync(customerId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le prenotazioni per una data specifica
        /// </summary>
        [HttpGet("by-date")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByDate(
            [FromQuery] DateTime date,
            [FromQuery] Guid? multitenantId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateAsync(date, multitenantId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for date {Date}", date);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le prenotazioni per un intervallo di date
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByDateRange(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] Guid? multitenantId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByDateRangeAsync(
                    fromDate, toDate, multitenantId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for date range");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le prenotazioni per un tecnico specifico
        /// </summary>
        [HttpGet("technician/{technicianCode}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByTechnician(
            string technicianCode,
            [FromQuery] Guid? multitenantId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByTechnicianAsync(
                    technicianCode, multitenantId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for technician {TechnicianCode}",
                    technicianCode);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene le prenotazioni per stato
        /// </summary>
        [HttpGet("by-status/{statusCode}")]
        public async Task<ActionResult<IEnumerable<BookingDetailDto>>> GetBookingsByStatus(
            string statusCode,
            [FromQuery] Guid? multitenantId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByStatusAsync(
                    statusCode, multitenantId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for status {StatusCode}", statusCode);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Aggiorna una prenotazione
        /// </summary>
        [HttpPut("{bookingId:guid}")]
        public async Task<ActionResult> UpdateBooking(
            Guid bookingId,
            [FromBody] UpdateBookingRequestDto request)
        {
            try
            {
                await _bookingService.UpdateBookingAsync(bookingId, request);
                return Ok(new { message = "Prenotazione aggiornata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId}", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Aggiorna lo stato di una prenotazione
        /// </summary>
        [HttpPut("{bookingId:guid}/status")]
        public async Task<ActionResult> UpdateBookingStatus(
            Guid bookingId,
            [FromBody] UpdateBookingStatusDto request)
        {
            try
            {
                await _bookingService.UpdateBookingStatusAsync(
                    bookingId,
                    request.StatusCode,
                    request.Status,
                    request.Notes);

                return Ok(new { message = "Stato aggiornato con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status {BookingId}", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Elimina una prenotazione (soft delete)
        /// </summary>
        [HttpDelete("{bookingId:guid}")]
        public async Task<ActionResult> DeleteBooking(Guid bookingId)
        {
            try
            {
                await _bookingService.DeleteBookingAsync(bookingId);
                return Ok(new { message = "Prenotazione eliminata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {BookingId}", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ripristina una prenotazione eliminata
        /// </summary>
        [HttpPost("{bookingId:guid}/restore")]
        public async Task<ActionResult> RestoreBooking(Guid bookingId)
        {
            try
            {
                await _bookingService.RestoreBookingAsync(bookingId);
                return Ok(new { message = "Prenotazione ripristinata con successo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring booking {BookingId}", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Conversione

        /// <summary>
        /// Converte una prenotazione in riparazione
        /// </summary>
        [HttpPost("{bookingId:guid}/convert-to-repair")]
        public async Task<ActionResult> ConvertToRepair(
            Guid bookingId,
            [FromBody] ConvertBookingToRepairDto? convertDto = null)
        {
            try
            {
                var repairId = await _bookingService.ConvertBookingToRepairAsync(
                    bookingId, convertDto);

                return Ok(new
                {
                    message = "Prenotazione convertita in riparazione con successo",
                    repairId = repairId,
                    bookingId = bookingId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting booking {BookingId} to repair", bookingId);
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Ottiene il numero di prenotazioni di oggi
        /// </summary>
        [HttpGet("stats/today-count")]
        public async Task<ActionResult<int>> GetTodayBookingsCount(
            [FromQuery] Guid multitenantId)
        {
            try
            {
                var count = await _bookingService.GetTodayBookingsCountAsync(multitenantId);
                return Ok(new { count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting today bookings count");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        /// <summary>
        /// Ottiene il numero di prenotazioni in attesa
        /// </summary>
        [HttpGet("stats/pending-count")]
        public async Task<ActionResult<int>> GetPendingBookingsCount(
            [FromQuery] Guid multitenantId)
        {
            try
            {
                var count = await _bookingService.GetPendingBookingsCountAsync(multitenantId);
                return Ok(new { count = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending bookings count");
                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }

        #endregion
    }
}