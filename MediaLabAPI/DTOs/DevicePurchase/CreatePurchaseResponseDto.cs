using System;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO di risposta dopo la creazione di un acquisto
    /// </summary>
    public class CreatePurchaseResponseDto
    {
        public int Id { get; set; }
        public Guid PurchaseId { get; set; }
        public string? PurchaseCode { get; set; }
        public string Message { get; set; } = "Acquisto creato con successo";
        public DateTime CreatedAt { get; set; }

        // Dati essenziali dell'acquisto creato
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string DeviceCondition { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PurchaseStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}
