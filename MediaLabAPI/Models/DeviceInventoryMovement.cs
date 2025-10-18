using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class DeviceInventoryMovement
    {
        public int Id { get; set; }

        [Required]
        public Guid MovementId { get; set; } = Guid.NewGuid(); // ✅ QUESTA PROPRIETÀ DEVE ESISTERE

        [Required]
        public int DeviceInventoryId { get; set; } // FK verso DeviceInventory.Id

        [Required]
        public Guid DeviceId { get; set; } // GUID denormalizzato

        [Required]
        [MaxLength(20)]
        public string MovementType { get; set; } = string.Empty; // purchase, loan, return, sale, transfer, status_change

        [MaxLength(20)]
        public string? FromStatus { get; set; }

        [MaxLength(20)]
        public string? ToStatus { get; set; }

        public Guid? CustomerId { get; set; }

        [MaxLength(200)]
        public string? Reference { get; set; }

        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        // Navigation Properties
        public virtual DeviceInventory? Device { get; set; }
        public virtual C_ANA_Company? Customer { get; set; }
    }
}