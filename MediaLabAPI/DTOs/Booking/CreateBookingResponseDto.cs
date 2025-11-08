namespace MediaLabAPI.DTOs.Booking
{
    public class CreateBookingResponseDto
    {
        public int Id { get; set; }
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string ScheduledTime { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}