using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Common
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username è obbligatorio")]
        [StringLength(100, ErrorMessage = "Username non può superare 100 caratteri")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password è obbligatoria")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password deve essere tra 6 e 256 caratteri")]
        public string Password { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email non valida")]
        [StringLength(100, ErrorMessage = "Email non può superare 100 caratteri")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "IdCompany è obbligatorio")]
        public Guid IdCompany { get; set; }

        public bool IsAdmin { get; set; } = false;

        [StringLength(50, ErrorMessage = "AccessLevel non può superare 50 caratteri")]
        public string? AccessLevel { get; set; }

        public Guid? IdWhr { get; set; }
    }
}