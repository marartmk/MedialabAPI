using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO per l'aggiornamento dello stato di un acquisto
    /// </summary>
    public class UpdatePurchaseStatusDto
    {
        [Required(ErrorMessage = "Lo stato dell'acquisto è obbligatorio")]
        [StringLength(50)]
        public string PurchaseStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il codice stato è obbligatorio")]
        [StringLength(20)]
        public string PurchaseStatusCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
