namespace MediaLabAPI.DTOs.Booking
{
    /// <summary>
    /// DTO leggero per liste di prenotazioni
    /// </summary>
    public class BookingListDto
    {
        public int Id { get; set; }
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string ScheduledTime { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public string BookingStatusCode { get; set; } = string.Empty;
        public string? TechnicianName { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public bool IsConverted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}