using System;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO di risposta dopo l'aggiunta di un pagamento all'acquisto
    /// </summary>
    public class AddPurchasePaymentResponseDto
    {
        /// <summary>
        /// ID univoco del pagamento
        /// </summary>
        public Guid PaymentId { get; set; }

        /// <summary>
        /// ID univoco dell'acquisto
        /// </summary>
        public Guid PurchaseId { get; set; }

        /// <summary>
        /// Importo del pagamento appena aggiunto
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Metodo di pagamento utilizzato
        /// </summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Data e ora del pagamento
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Nuovo importo totale pagato dopo questo pagamento
        /// </summary>
        public decimal NewPaidAmount { get; set; }

        /// <summary>
        /// Nuovo importo residuo da pagare
        /// </summary>
        public decimal NewRemainingAmount { get; set; }

        /// <summary>
        /// Nuovo stato del pagamento ("Pagato", "Parzialmente Pagato", "Da Pagare")
        /// </summary>
        public string NewPaymentStatus { get; set; } = string.Empty;

        /// <summary>
        /// Messaggio di conferma
        /// </summary>
        public string Message { get; set; } = "Pagamento registrato con successo";
    }
}
