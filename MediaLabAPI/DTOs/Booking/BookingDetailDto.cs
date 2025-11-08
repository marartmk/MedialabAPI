namespace MediaLabAPI.DTOs.Booking
{
    public class BookingDetailDto
    {
        public int Id { get; set; }
        public Guid BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;

        // Cliente
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }

        // Dispositivo
        public string? DeviceType { get; set; }
        public string? DeviceBrand { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceColor { get; set; }

        // Prenotazione
        public DateTime BookingDateTime { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string ScheduledTime { get; set; } = string.Empty;

        // Tecnico
        public string? TechnicianCode { get; set; }
        public string? TechnicianName { get; set; }

        // Problema
        public string? ComponentIssue { get; set; }
        public string? ProblemDescription { get; set; }

        // Preventivo
        public decimal? EstimatedPrice { get; set; }
        public string? PaymentType { get; set; }
        public string? BillingInfo { get; set; }

        // Stato
        public string BookingStatus { get; set; } = string.Empty;
        public string BookingStatusCode { get; set; } = string.Empty;

        // Note
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Conversione
        public Guid? ConvertedToRepairId { get; set; }
        public DateTime? ConvertedAt { get; set; }
        public bool IsConverted => ConvertedToRepairId.HasValue;
    }
}