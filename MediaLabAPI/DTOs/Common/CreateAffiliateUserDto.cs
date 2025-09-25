using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Common
{
    public class CreateAffiliateUserDto
    {
        // Dati utente
        [Required(ErrorMessage = "Username è obbligatorio")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password è obbligatoria")]
        [StringLength(256, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        // Dati customer/company collegato
        [Required(ErrorMessage = "IdCustomer è obbligatorio")]
        public Guid IdCustomer { get; set; }

        // Livello di accesso predefinito per affiliati
        public string AccessLevel { get; set; } = "Affiliate";
    }
}
