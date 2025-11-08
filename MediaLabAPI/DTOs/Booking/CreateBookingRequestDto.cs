using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Booking
{
    public class CreateBookingRequestDto
    {
        // Cliente (opzionale - può essere nuovo o esistente)
        public Guid? CustomerId { get; set; }

        [StringLength(200)]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? CustomerEmail { get; set; }

        // Dispositivo
        [Required(ErrorMessage = "Il tipo di dispositivo è obbligatorio")]
        [StringLength(50)]
        public string DeviceType { get; set; } = string.Empty;

        [StringLength(100)]
        public string? DeviceBrand { get; set; }

        [Required(ErrorMessage = "Il modello è obbligatorio")]
        [StringLength(100)]
        public string DeviceModel { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DeviceColor { get; set; }

        // Data e ora prenotazione
        [Required(ErrorMessage = "Data e ora sono obbligatorie")]
        public DateTime BookingDateTime { get; set; }

        // Tecnico
        [StringLength(50)]
        public string? TechnicianCode { get; set; }

        [StringLength(100)]
        public string? TechnicianName { get; set; }

        // Problema
        [StringLength(200)]
        public string? ComponentIssue { get; set; }

        [Required(ErrorMessage = "La descrizione del problema è obbligatoria")]
        [StringLength(1000)]
        public string ProblemDescription { get; set; } = string.Empty;

        // Preventivo
        [Range(0.01, 999999.99, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal? EstimatedPrice { get; set; }

        [StringLength(50)]
        public string? PaymentType { get; set; }

        // Fatturazione
        [StringLength(500)]
        public string? BillingInfo { get; set; }

        // Contesto
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        // Note
        [StringLength(2000)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}