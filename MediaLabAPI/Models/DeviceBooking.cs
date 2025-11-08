using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("DeviceBookings")]
    public class DeviceBooking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid BookingId { get; set; } = Guid.NewGuid();

        [StringLength(50)]
        public string? BookingCode { get; set; }

        // Cliente
        public Guid? CustomerId { get; set; }

        [StringLength(200)]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? CustomerPhone { get; set; }

        [StringLength(100)]
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

        // Prenotazione
        [Required]
        public DateTime BookingDateTime { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        public TimeSpan ScheduledTime { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedPrice { get; set; }

        [StringLength(50)]
        public string? PaymentType { get; set; }

        // Fatturazione
        [StringLength(500)]
        public string? BillingInfo { get; set; }

        // Stato
        [Required]
        [StringLength(50)]
        public string BookingStatus { get; set; } = "Confermata";

        [Required]
        [StringLength(20)]
        public string BookingStatusCode { get; set; } = "CONFIRMED";

        // Multitenant
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        // Note
        [StringLength(2000)]
        public string? Notes { get; set; }

        // Audit
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Conversione
        public Guid? ConvertedToRepairId { get; set; }
        public DateTime? ConvertedAt { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual C_ANA_Company? Customer { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }
    }
}