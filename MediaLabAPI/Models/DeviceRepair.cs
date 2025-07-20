using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("DeviceRepairs")]
    public class DeviceRepair
    {
        [Key]
        public int Id { get; set; }

        // 🆕 NUOVO: RepairId univoco per ogni riparazione
        [Required]
        public Guid RepairId { get; set; } = Guid.NewGuid();

        // 🆕 NUOVO: Codice riparazione ricercabile
        [StringLength(50)]
        public string? RepairCode { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        public Guid? CustomerId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        [Required]
        [StringLength(500)]
        public string FaultDeclared { get; set; } = string.Empty;

        [StringLength(500)]
        public string? FaultDetected { get; set; }

        [StringLength(500)]
        public string? RepairAction { get; set; }

        [Required]
        [StringLength(20)]
        public string RepairStatusCode { get; set; } = "RECEIVED";

        [Required]
        [StringLength(100)]
        public string RepairStatus { get; set; } = "Ricevuto";

        [StringLength(50)]
        public string? TechnicianCode { get; set; }

        [StringLength(100)]
        public string? TechnicianName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReceivedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey("DeviceId")]
        public virtual DeviceRegistry? Device { get; set; }

        [ForeignKey("CustomerId")]
        public virtual C_ANA_Company? Customer { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }

        // 🆕 NUOVO: Relazione con test di ingresso
        public virtual IncomingTest? IncomingTest { get; set; }
    }
}