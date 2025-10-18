using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class ChangeDeviceStatusDto
    {
        [Required]
        [RegularExpression("^(available|loaned|sold|unavailable)$", ErrorMessage = "Stato non valido")]
        public string NewStatus { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }
}