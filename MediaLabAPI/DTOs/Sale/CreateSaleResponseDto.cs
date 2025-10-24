using System;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO di risposta dopo la creazione di una vendita
    /// </summary>
    public class CreateSaleResponseDto
    {
        public int Id { get; set; }
        public Guid SaleId { get; set; }
        public string? SaleCode { get; set; }
        public string Message { get; set; } = "Vendita creata con successo";
        public DateTime CreatedAt { get; set; }

        // Dati essenziali della vendita creata
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string SaleStatus { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}