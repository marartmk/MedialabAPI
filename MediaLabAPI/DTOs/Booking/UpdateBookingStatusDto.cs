using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Booking
{
    public class UpdateBookingStatusDto
    {
        [Required(ErrorMessage = "Il codice stato è obbligatorio")]
        [StringLength(20)]
        public string StatusCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lo stato è obbligatorio")]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}