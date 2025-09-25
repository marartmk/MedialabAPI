using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class WarehouseSupplier
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string SupplierId { get; set; } = string.Empty; // ID fornitore

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // Nome fornitore

        [MaxLength(200)]
        public string? Contact { get; set; } // Contatto (email/telefono)

        [MaxLength(500)]
        public string? Address { get; set; } // Indirizzo

        [MaxLength(100)]
        public string? Phone { get; set; } // Telefono

        [MaxLength(100)]
        public string? Email { get; set; } // Email

        [MaxLength(1000)]
        public string? Notes { get; set; } // Note

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual C_ANA_Company? Company { get; set; }
        public virtual ICollection<WarehouseItem> Items { get; set; } = new List<WarehouseItem>();
    }
}
