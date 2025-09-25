using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per le informazioni della categoria
    public class CategoryInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }
}