using System;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO con i dettagli di un pagamento
    /// </summary>
    public class SalePaymentDetailDto
    {
        public int Id { get; set; }
        public Guid PaymentId { get; set; }
        public Guid SaleId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public string? ReceivedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}