using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class ReturnDeviceDto
    {
        [Required]
        [RegularExpression("^(available|unavailable)$", ErrorMessage = "Stato dopo restituzione non valido")]
        public string ReturnStatus { get; set; } = "available";

        public string? Notes { get; set; }
        public string? Condition { get; set; }
    }
}