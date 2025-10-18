using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class DeviceInventorySupplier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SupplierId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Contact { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Notes { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual C_ANA_Company? Company { get; set; }
        public virtual ICollection<DeviceInventory> Devices { get; set; } = new List<DeviceInventory>();
    }
}