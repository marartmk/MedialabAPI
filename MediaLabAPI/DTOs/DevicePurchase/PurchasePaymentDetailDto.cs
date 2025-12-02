using System;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO con i dettagli di un pagamento di acquisto
    /// </summary>
    public class PurchasePaymentDetailDto
    {
        public int Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid PurchaseId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public string? PaidBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
