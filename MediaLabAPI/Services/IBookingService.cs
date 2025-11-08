using MediaLabAPI.DTOs.Booking;

namespace MediaLabAPI.Services
{
    public interface IBookingService
    {
        // Create
        Task<CreateBookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request);

        // Read
        Task<BookingDetailDto?> GetBookingByIdAsync(int id);
        Task<BookingDetailDto?> GetBookingByGuidAsync(Guid bookingId);
        Task<BookingDetailDto?> GetBookingByCodeAsync(string bookingCode);
        Task<IEnumerable<BookingDetailDto>> GetAllBookingsAsync(Guid? multitenantId);
        Task<IEnumerable<BookingListDto>> SearchBookingsAsync(BookingSearchRequestDto searchRequest);

        // Filtri specifici
        Task<IEnumerable<BookingDetailDto>> GetBookingsByCustomerAsync(Guid customerId);
        Task<IEnumerable<BookingDetailDto>> GetBookingsByDateAsync(DateTime date, Guid? multitenantId);
        Task<IEnumerable<BookingDetailDto>> GetBookingsByDateRangeAsync(DateTime fromDate, DateTime toDate, Guid? multitenantId);
        Task<IEnumerable<BookingDetailDto>> GetBookingsByTechnicianAsync(string technicianCode, Guid? multitenantId);
        Task<IEnumerable<BookingDetailDto>> GetBookingsByStatusAsync(string statusCode, Guid? multitenantId);

        // Update
        Task UpdateBookingAsync(Guid bookingId, UpdateBookingRequestDto request);
        Task UpdateBookingStatusAsync(Guid bookingId, string statusCode, string status, string? notes = null);

        // Delete
        Task DeleteBookingAsync(Guid bookingId);
        Task RestoreBookingAsync(Guid bookingId);

        // Conversione
        Task<Guid> ConvertBookingToRepairAsync(Guid bookingId, ConvertBookingToRepairDto? convertDto = null);

        // Statistiche
        Task<int> GetTodayBookingsCountAsync(Guid multitenantId);
        Task<int> GetPendingBookingsCountAsync(Guid multitenantId);
    }
}