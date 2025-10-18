using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class CreateDeviceInventorySupplierDto
    {
        [Required(ErrorMessage = "L'ID fornitore è obbligatorio")]
        [MaxLength(50)]
        public string SupplierId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il nome è obbligatorio")]
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
        public Guid MultitenantId { get; set; }
    }
}