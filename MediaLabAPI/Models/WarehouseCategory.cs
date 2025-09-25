using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class WarehouseCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryId { get; set; } = string.Empty; // ID categoria (screens, batteries, etc.)

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // Nome categoria

        [MaxLength(10)]
        public string? Icon { get; set; } // Icona emoji

        [MaxLength(10)]
        public string? Color { get; set; } // Colore esadecimale

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual C_ANA_Company? Company { get; set; }
    }
}