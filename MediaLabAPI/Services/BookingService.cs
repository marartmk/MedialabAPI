using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.DTOs.Booking;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Create

        public async Task<CreateBookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request)
        {
            // Validazione base
            if (string.IsNullOrWhiteSpace(request.DeviceModel))
                throw new ArgumentException("Il modello del dispositivo è obbligatorio");

            if (string.IsNullOrWhiteSpace(request.ProblemDescription))
                throw new ArgumentException("La descrizione del problema è obbligatoria");

            // Genera codice prenotazione univoco
            var bookingCode = await GenerateBookingCodeAsync();

            // Estrai data e ora
            var scheduledDate = request.BookingDateTime.Date;
            var scheduledTime = request.BookingDateTime.TimeOfDay;

            // Crea entità
            var booking = new DeviceBooking
            {
                BookingId = Guid.NewGuid(),
                BookingCode = bookingCode,

                // Cliente
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                CustomerEmail = request.CustomerEmail,

                // Dispositivo
                DeviceType = request.DeviceType,
                DeviceBrand = request.DeviceBrand,
                DeviceModel = request.DeviceModel,
                DeviceColor = request.DeviceColor,

                // Prenotazione
                BookingDateTime = request.BookingDateTime,
                ScheduledDate = scheduledDate,
                ScheduledTime = scheduledTime,

                // Tecnico
                TechnicianCode = request.TechnicianCode,
                TechnicianName = request.TechnicianName,

                // Problema
                ComponentIssue = request.ComponentIssue,
                ProblemDescription = request.ProblemDescription,

                // Preventivo
                EstimatedPrice = request.EstimatedPrice,
                PaymentType = request.PaymentType,
                BillingInfo = request.BillingInfo,

                // Stato
                BookingStatus = "Prenotata",
                BookingStatusCode = "BOOKED",

                // Contesto
                CompanyId = request.CompanyId,
                MultitenantId = request.MultitenantId,

                // Note
                Notes = request.Notes,

                // Audit
                CreatedAt = DateTime.Now,
                CreatedBy = request.CreatedBy,
                IsDeleted = false
            };

            _context.DeviceBookings.Add(booking);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created booking {BookingCode} for customer {CustomerName}",
                bookingCode, request.CustomerName);

            return new CreateBookingResponseDto
            {
                Id = booking.Id,
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                CustomerName = booking.CustomerName ?? "Cliente generico",
                ScheduledDate = booking.ScheduledDate,
                ScheduledTime = booking.ScheduledTime.ToString(@"hh\:mm"),
                DeviceType = booking.DeviceType ?? "",
                DeviceModel = booking.DeviceModel,
                BookingStatus = booking.BookingStatus,
                Message = "Prenotazione creata con successo",
                CreatedAt = booking.CreatedAt
            };
        }

        private async Task<string> GenerateBookingCodeAsync()
        {
            var now = DateTime.Now;
            var year = now.Year.ToString().Substring(2, 2);
            var month = now.Month.ToString("D2");
            var day = now.Day.ToString("D2");

            // Genera numero casuale a 6 cifre per ridurre collisioni
            var timestamp = now.Ticks.ToString().Substring(now.Ticks.ToString().Length - 6);
            var random = new Random().Next(0, 100).ToString("D2");

            return $"BOOK-{year}{month}{day}-{timestamp}{random}";
        }

        #endregion

        #region Read

        public async Task<BookingDetailDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);

            return booking == null ? null : MapToDetailDto(booking);
        }

        public async Task<BookingDetailDto?> GetBookingByGuidAsync(Guid bookingId)
        {
            var booking = await _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && !b.IsDeleted);

            return booking == null ? null : MapToDetailDto(booking);
        }

        public async Task<BookingDetailDto?> GetBookingByCodeAsync(string bookingCode)
        {
            var booking = await _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.BookingCode == bookingCode && !b.IsDeleted);

            return booking == null ? null : MapToDetailDto(booking);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetAllBookingsAsync(Guid? multitenantId)
        {
            var query = _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => !b.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == multitenantId.Value);

            var bookings = await query
                .OrderByDescending(b => b.ScheduledDate)
                .ThenBy(b => b.ScheduledTime)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        public async Task<IEnumerable<BookingListDto>> SearchBookingsAsync(BookingSearchRequestDto searchRequest)
        {
            var query = _context.DeviceBookings
                .Where(b => !b.IsDeleted)
                .AsQueryable();

            // Filtro tenant
            if (searchRequest.MultitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == searchRequest.MultitenantId.Value);

            // Ricerca testuale generale
            if (!string.IsNullOrWhiteSpace(searchRequest.SearchQuery))
            {
                var searchLower = searchRequest.SearchQuery.ToLower();
                query = query.Where(b =>
                    b.BookingCode!.ToLower().Contains(searchLower) ||
                    b.CustomerName!.ToLower().Contains(searchLower) ||
                    b.CustomerPhone!.ToLower().Contains(searchLower) ||
                    b.DeviceModel!.ToLower().Contains(searchLower));
            }

            // Filtri specifici
            if (!string.IsNullOrWhiteSpace(searchRequest.BookingCode))
                query = query.Where(b => b.BookingCode == searchRequest.BookingCode);

            if (searchRequest.BookingGuid.HasValue)
                query = query.Where(b => b.BookingId == searchRequest.BookingGuid.Value);

            if (searchRequest.CustomerId.HasValue)
                query = query.Where(b => b.CustomerId == searchRequest.CustomerId.Value);

            if (!string.IsNullOrWhiteSpace(searchRequest.StatusCode))
                query = query.Where(b => b.BookingStatusCode == searchRequest.StatusCode);

            if (!string.IsNullOrWhiteSpace(searchRequest.TechnicianCode))
                query = query.Where(b => b.TechnicianCode == searchRequest.TechnicianCode);

            // Filtri temporali
            if (searchRequest.FromDate.HasValue)
                query = query.Where(b => b.ScheduledDate >= searchRequest.FromDate.Value.Date);

            if (searchRequest.ToDate.HasValue)
                query = query.Where(b => b.ScheduledDate <= searchRequest.ToDate.Value.Date);

            if (searchRequest.ScheduledDate.HasValue)
                query = query.Where(b => b.ScheduledDate == searchRequest.ScheduledDate.Value.Date);

            // Filtri dispositivo
            if (!string.IsNullOrWhiteSpace(searchRequest.DeviceType))
                query = query.Where(b => b.DeviceType == searchRequest.DeviceType);

            if (!string.IsNullOrWhiteSpace(searchRequest.DeviceBrand))
                query = query.Where(b => b.DeviceBrand == searchRequest.DeviceBrand);

            if (!string.IsNullOrWhiteSpace(searchRequest.DeviceModel))
                query = query.Where(b => b.DeviceModel!.ToLower().Contains(searchRequest.DeviceModel.ToLower()));

            // Filtro conversione
            if (searchRequest.IsConverted.HasValue)
            {
                if (searchRequest.IsConverted.Value)
                    query = query.Where(b => b.ConvertedToRepairId != null);
                else
                    query = query.Where(b => b.ConvertedToRepairId == null);
            }

            // Ordinamento
            query = searchRequest.SortBy?.ToLower() switch
            {
                "bookingcode" => searchRequest.SortDescending
                    ? query.OrderByDescending(b => b.BookingCode)
                    : query.OrderBy(b => b.BookingCode),
                "customername" => searchRequest.SortDescending
                    ? query.OrderByDescending(b => b.CustomerName)
                    : query.OrderBy(b => b.CustomerName),
                "createdat" => searchRequest.SortDescending
                    ? query.OrderByDescending(b => b.CreatedAt)
                    : query.OrderBy(b => b.CreatedAt),
                _ => searchRequest.SortDescending
                    ? query.OrderByDescending(b => b.ScheduledDate).ThenByDescending(b => b.ScheduledTime)
                    : query.OrderBy(b => b.ScheduledDate).ThenBy(b => b.ScheduledTime)
            };

            // Paginazione
            var validPage = searchRequest.GetValidPage();
            var validPageSize = searchRequest.GetValidPageSize();

            var bookings = await query
                .Skip((validPage - 1) * validPageSize)
                .Take(validPageSize)
                .ToListAsync();

            return bookings.Select(MapToListDto);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetBookingsByCustomerAsync(Guid customerId)
        {
            var bookings = await _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => b.CustomerId == customerId && !b.IsDeleted)
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetBookingsByDateAsync(DateTime date, Guid? multitenantId)
        {
            var query = _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => b.ScheduledDate == date.Date && !b.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == multitenantId.Value);

            var bookings = await query
                .OrderBy(b => b.ScheduledTime)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetBookingsByDateRangeAsync(
            DateTime fromDate, DateTime toDate, Guid? multitenantId)
        {
            var query = _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => b.ScheduledDate >= fromDate.Date &&
                           b.ScheduledDate <= toDate.Date &&
                           !b.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == multitenantId.Value);

            var bookings = await query
                .OrderBy(b => b.ScheduledDate)
                .ThenBy(b => b.ScheduledTime)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetBookingsByTechnicianAsync(
            string technicianCode, Guid? multitenantId)
        {
            var query = _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => b.TechnicianCode == technicianCode && !b.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == multitenantId.Value);

            var bookings = await query
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        public async Task<IEnumerable<BookingDetailDto>> GetBookingsByStatusAsync(
            string statusCode, Guid? multitenantId)
        {
            var query = _context.DeviceBookings
                .Include(b => b.Customer)
                .Include(b => b.Company)
                .Where(b => b.BookingStatusCode == statusCode && !b.IsDeleted);

            if (multitenantId.HasValue)
                query = query.Where(b => b.MultitenantId == multitenantId.Value);

            var bookings = await query
                .OrderByDescending(b => b.ScheduledDate)
                .ToListAsync();

            return bookings.Select(MapToDetailDto);
        }

        #endregion

        #region Update

        public async Task UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request)
        {
            var booking = await _context.DeviceBookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && !b.IsDeleted);

            if (booking == null)
                throw new ArgumentException("Prenotazione non trovata");

            // Aggiorna solo i campi forniti
            if (request.CustomerId.HasValue)
                booking.CustomerId = request.CustomerId.Value;

            if (!string.IsNullOrWhiteSpace(request.CustomerName))
                booking.CustomerName = request.CustomerName;

            if (!string.IsNullOrWhiteSpace(request.CustomerPhone))
                booking.CustomerPhone = request.CustomerPhone;

            if (!string.IsNullOrWhiteSpace(request.CustomerEmail))
                booking.CustomerEmail = request.CustomerEmail;

            if (!string.IsNullOrWhiteSpace(request.DeviceType))
                booking.DeviceType = request.DeviceType;

            if (!string.IsNullOrWhiteSpace(request.DeviceBrand))
                booking.DeviceBrand = request.DeviceBrand;

            if (!string.IsNullOrWhiteSpace(request.DeviceModel))
                booking.DeviceModel = request.DeviceModel;

            if (!string.IsNullOrWhiteSpace(request.DeviceColor))
                booking.DeviceColor = request.DeviceColor;

            if (request.BookingDateTime.HasValue)
            {
                booking.BookingDateTime = request.BookingDateTime.Value;
                booking.ScheduledDate = request.BookingDateTime.Value.Date;
                booking.ScheduledTime = request.BookingDateTime.Value.TimeOfDay;
            }

            if (!string.IsNullOrWhiteSpace(request.TechnicianCode))
                booking.TechnicianCode = request.TechnicianCode;

            if (!string.IsNullOrWhiteSpace(request.TechnicianName))
                booking.TechnicianName = request.TechnicianName;

            if (!string.IsNullOrWhiteSpace(request.ComponentIssue))
                booking.ComponentIssue = request.ComponentIssue;

            if (!string.IsNullOrWhiteSpace(request.ProblemDescription))
                booking.ProblemDescription = request.ProblemDescription;

            if (request.EstimatedPrice.HasValue)
                booking.EstimatedPrice = request.EstimatedPrice.Value;

            if (!string.IsNullOrWhiteSpace(request.PaymentType))
                booking.PaymentType = request.PaymentType;

            if (!string.IsNullOrWhiteSpace(request.BillingInfo))
                booking.BillingInfo = request.BillingInfo;

            if (!string.IsNullOrWhiteSpace(request.Notes))
                booking.Notes = request.Notes;

            booking.UpdatedAt = DateTime.Now;
            booking.UpdatedBy = request.UpdatedBy;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated booking {BookingCode}", booking.BookingCode);
        }

        public async Task UpdateBookingStatusAsync(Guid bookingId, string statusCode, string status, string? notes = null)
        {
            var booking = await _context.DeviceBookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && !b.IsDeleted);

            if (booking == null)
                throw new ArgumentException("Prenotazione non trovata");

            booking.BookingStatusCode = statusCode;
            booking.BookingStatus = status;

            if (!string.IsNullOrWhiteSpace(notes))
            {
                booking.Notes = string.IsNullOrWhiteSpace(booking.Notes)
                    ? notes
                    : $"{booking.Notes}\n---\n{notes}";
            }

            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated booking {BookingCode} status to {Status}",
                booking.BookingCode, status);
        }

        #endregion

        #region Delete

        public async Task DeleteBookingAsync(Guid bookingId)
        {
            var booking = await _context.DeviceBookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new ArgumentException("Prenotazione non trovata");

            booking.IsDeleted = true;
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted booking {BookingCode}", booking.BookingCode);
        }

        public async Task RestoreBookingAsync(Guid bookingId)
        {
            var booking = await _context.DeviceBookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                throw new ArgumentException("Prenotazione non trovata");

            booking.IsDeleted = false;
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Restored booking {BookingCode}", booking.BookingCode);
        }

        #endregion

        #region Conversione

        public async Task<Guid> ConvertBookingToRepairAsync(Guid bookingId, ConvertBookingToRepairDto? convertDto = null)
        {
            var booking = await _context.DeviceBookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId && !b.IsDeleted);

            if (booking == null)
                throw new ArgumentException("Prenotazione non trovata");

            if (booking.ConvertedToRepairId.HasValue)
                throw new ArgumentException("Questa prenotazione è già stata convertita in riparazione");

            // TODO: Implementare logica di conversione a DeviceRepair
            // Per ora generiamo solo un GUID placeholder
            var repairId = Guid.NewGuid();

            booking.ConvertedToRepairId = repairId;
            booking.ConvertedAt = DateTime.Now;
            booking.BookingStatusCode = "CONVERTED";
            booking.BookingStatus = "Convertita in Riparazione";
            booking.UpdatedAt = DateTime.Now;

            if (convertDto?.AdditionalNotes != null)
            {
                booking.Notes = string.IsNullOrWhiteSpace(booking.Notes)
                    ? convertDto.AdditionalNotes
                    : $"{booking.Notes}\n---\n{convertDto.AdditionalNotes}";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Converted booking {BookingCode} to repair {RepairId}",
                booking.BookingCode, repairId);

            return repairId;
        }

        #endregion

        #region Statistiche

        public async Task<int> GetTodayBookingsCountAsync(Guid multitenantId)
        {
            var today = DateTime.Today;
            return await _context.DeviceBookings
                .CountAsync(b => b.ScheduledDate == today &&
                               b.MultitenantId == multitenantId &&
                               !b.IsDeleted);
        }

        public async Task<int> GetPendingBookingsCountAsync(Guid multitenantId)
        {
            var today = DateTime.Today;
            return await _context.DeviceBookings
                .CountAsync(b => b.ScheduledDate >= today &&
                               b.BookingStatusCode == "CONFIRMED" &&
                               b.MultitenantId == multitenantId &&
                               !b.IsDeleted);
        }

        #endregion

        #region Mapping

        private BookingDetailDto MapToDetailDto(DeviceBooking booking)
        {
            return new BookingDetailDto
            {
                Id = booking.Id,
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode ?? "",
                CustomerId = booking.CustomerId,
                CustomerName = booking.CustomerName,
                CustomerPhone = booking.CustomerPhone,
                CustomerEmail = booking.CustomerEmail,
                DeviceType = booking.DeviceType,
                DeviceBrand = booking.DeviceBrand,
                DeviceModel = booking.DeviceModel,
                DeviceColor = booking.DeviceColor,
                BookingDateTime = booking.BookingDateTime,
                ScheduledDate = booking.ScheduledDate,
                ScheduledTime = booking.ScheduledTime.ToString(@"hh\:mm"),
                TechnicianCode = booking.TechnicianCode,
                TechnicianName = booking.TechnicianName,
                ComponentIssue = booking.ComponentIssue,
                ProblemDescription = booking.ProblemDescription,
                EstimatedPrice = booking.EstimatedPrice,
                PaymentType = booking.PaymentType,
                BillingInfo = booking.BillingInfo,
                BookingStatus = booking.BookingStatus,
                BookingStatusCode = booking.BookingStatusCode,
                Notes = booking.Notes,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                CreatedBy = booking.CreatedBy,
                UpdatedBy = booking.UpdatedBy,
                ConvertedToRepairId = booking.ConvertedToRepairId,
                ConvertedAt = booking.ConvertedAt
            };
        }

        private BookingListDto MapToListDto(DeviceBooking booking)
        {
            return new BookingListDto
            {
                Id = booking.Id,
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode ?? "",
                CustomerName = booking.CustomerName ?? "N/A",
                CustomerPhone = booking.CustomerPhone ?? "",
                DeviceModel = booking.DeviceModel ?? "",
                ScheduledDate = booking.ScheduledDate,
                ScheduledTime = booking.ScheduledTime.ToString(@"hh\:mm"),
                BookingStatus = booking.BookingStatus,
                BookingStatusCode = booking.BookingStatusCode,
                TechnicianName = booking.TechnicianName,
                EstimatedPrice = booking.EstimatedPrice,
                IsConverted = booking.ConvertedToRepairId.HasValue,
                CreatedAt = booking.CreatedAt
            };
        }

        #endregion
    }
}