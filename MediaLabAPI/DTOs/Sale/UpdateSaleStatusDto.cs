using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO per l'aggiornamento dello stato di una vendita
    /// </summary>
    public class UpdateSaleStatusDto
    {
        [Required(ErrorMessage = "Lo stato della vendita è obbligatorio")]
        [StringLength(50)]
        public string SaleStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il codice stato è obbligatorio")]
        [StringLength(20)]
        public string SaleStatusCode { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}