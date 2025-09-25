using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Common
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Nuova password è obbligatoria")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Password deve essere tra 6 e 256 caratteri")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Le password non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}