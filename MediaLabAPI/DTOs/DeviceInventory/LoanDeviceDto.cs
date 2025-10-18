using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class LoanDeviceDto
    {
        [Required(ErrorMessage = "Il cliente è obbligatorio")]
        public Guid CustomerId { get; set; }

        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
    }
}