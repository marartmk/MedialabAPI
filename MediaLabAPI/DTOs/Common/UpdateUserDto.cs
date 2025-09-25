using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Common
{
    public class UpdateUserDto
    {
        [EmailAddress(ErrorMessage = "Email non valida")]
        [StringLength(100, ErrorMessage = "Email non può superare 100 caratteri")]
        public string? Email { get; set; }

        public bool? IsEnabled { get; set; }

        public bool? IsAdmin { get; set; }

        [StringLength(50, ErrorMessage = "AccessLevel non può superare 50 caratteri")]
        public string? AccessLevel { get; set; }
    }
}