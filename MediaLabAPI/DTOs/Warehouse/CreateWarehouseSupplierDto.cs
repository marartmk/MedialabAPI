using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la creazione di un fornitore
    public class CreateWarehouseSupplierDto
    {
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

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }
    }
}