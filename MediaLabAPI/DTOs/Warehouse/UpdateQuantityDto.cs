using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per aggiornamento quantità
    public class UpdateQuantityDto
    {
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string Action { get; set; } = string.Empty; // add, remove, set
    }
}