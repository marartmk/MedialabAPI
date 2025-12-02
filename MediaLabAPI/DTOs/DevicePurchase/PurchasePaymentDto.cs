using System;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO per aggiungere un pagamento a un acquisto
    /// </summary>
    public class PurchasePaymentDto
    {
        [Required(ErrorMessage = "L'importo è obbligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "L'importo deve essere maggiore di 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Il metodo di pagamento è obbligatorio")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public DateTime? PaymentDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? PaidBy { get; set; }
    }
}
