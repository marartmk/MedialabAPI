using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la creazione di una categoria
    public class CreateWarehouseCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Icon { get; set; }

        [MaxLength(10)]
        public string? Color { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }
    }
}