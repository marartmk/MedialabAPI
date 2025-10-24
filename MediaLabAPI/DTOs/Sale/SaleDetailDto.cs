using System;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO con i dettagli completi di una vendita
    /// </summary>
    public class SaleDetailDto
    {
        public int Id { get; set; }
        public Guid SaleId { get; set; }
        public string? SaleCode { get; set; }

        // Tipo vendita
        public string SaleType { get; set; } = "Apparato";

        // IDs prodotto
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // Dati prodotto
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public string? IMEI { get; set; }

        // IDs relazionali
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid MultitenantId { get; set; }

        // Prezzi
        public decimal SalePrice { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal VatRate { get; set; }
        public decimal TotalAmount { get; set; }

        // Pagamento
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Da Pagare";
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int? InstallmentsCount { get; set; }
        public decimal? InstallmentAmount { get; set; }

        // Stato vendita
        public string SaleStatus { get; set; } = "Bozza";
        public string SaleStatusCode { get; set; } = "DRAFT";

        // Documenti
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? ReceiptId { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime? ReceiptDate { get; set; }

        // Venditore
        public string? SellerCode { get; set; }
        public string? SellerName { get; set; }

        // Note e accessori
        public string? Notes { get; set; }
        public string? IncludedAccessories { get; set; }

        // Garanzia
        public bool HasWarranty { get; set; }
        public int? WarrantyMonths { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }

        // Date
        public DateTime CreatedAt { get; set; }
        public DateTime? SaleDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Metadata
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Lista pagamenti (opzionale)
        public List<SalePaymentDetailDto>? Payments { get; set; }
    }
}