using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.RepairParts
{
    /// <summary>
    /// DTO per aggiornare un ricambio esistente
    /// </summary>
    public class UpdateRepairPartDto
    {
        [Required(ErrorMessage = "La quantità è obbligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Il prezzo unitario deve essere >= 0")]
        public decimal? UnitPrice { get; set; } // Opzionale per override

        [MaxLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        public string? Notes { get; set; }
    }
}