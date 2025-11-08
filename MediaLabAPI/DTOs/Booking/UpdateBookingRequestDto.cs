using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Booking
{
    public class UpdateBookingRequestDto
    {
        // Cliente
        public Guid? CustomerId { get; set; }

        [StringLength(200)]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? CustomerEmail { get; set; }

        // Dispositivo
        [StringLength(50)]
        public string? DeviceType { get; set; }

        [StringLength(100)]
        public string? DeviceBrand { get; set; }

        [StringLength(100)]
        public string? DeviceModel { get; set; }

        [StringLength(50)]
        public string? DeviceColor { get; set; }

        // Data e ora prenotazione
        public DateTime? BookingDateTime { get; set; }

        // Tecnico
        [StringLength(50)]
        public string? TechnicianCode { get; set; }

        [StringLength(100)]
        public string? TechnicianName { get; set; }

        // Problema
        [StringLength(200)]
        public string? ComponentIssue { get; set; }

        [StringLength(1000)]
        public string? ProblemDescription { get; set; }

        // Preventivo
        [Range(0.01, 999999.99)]
        public decimal? EstimatedPrice { get; set; }

        [StringLength(50)]
        public string? PaymentType { get; set; }

        // Fatturazione
        [StringLength(500)]
        public string? BillingInfo { get; set; }

        // Note
        [StringLength(2000)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}