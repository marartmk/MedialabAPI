using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per il movimento di magazzino
    public class WarehouseMovementDto
    {
        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public string MovementType { get; set; } = string.Empty; // in, out, adjustment

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(200)]
        public string? Reference { get; set; } // Numero ordine, fattura, etc.

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime? Date { get; set; }
    }
}