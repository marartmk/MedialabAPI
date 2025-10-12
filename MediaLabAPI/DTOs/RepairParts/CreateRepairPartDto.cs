using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per creare un nuovo ricambio associato a una riparazione
    /// </summary>
    public class CreateRepairPartDto
    {
        [Required(ErrorMessage = "Il WarehouseItemId è obbligatorio")]
        public int WarehouseItemId { get; set; }

        [Required(ErrorMessage = "La quantità è obbligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1")]
        public int Quantity { get; set; }

        [MaxLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        public string? Notes { get; set; }
    }
}