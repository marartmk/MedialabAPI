using System;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO per la creazione di una nuova vendita
    /// </summary>
    public class CreateSaleRequestDto
    {
        // Tipo vendita
        [Required(ErrorMessage = "Il tipo vendita è obbligatorio")]
        [StringLength(20)]
        public string SaleType { get; set; } = "Apparato";

        // IDs prodotto
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // Dati prodotto
        [Required(ErrorMessage = "La marca è obbligatoria")]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il modello è obbligatorio")]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [StringLength(50)]
        public string? IMEI { get; set; }

        // IDs relazionali
        [Required(ErrorMessage = "Il CustomerId è obbligatorio")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "Il CompanyId è obbligatorio")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Il MultitenantId è obbligatorio")]
        public Guid MultitenantId { get; set; }

        // Prezzi
        [Required(ErrorMessage = "Il prezzo di vendita è obbligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal SalePrice { get; set; }

        public decimal? OriginalPrice { get; set; }
        public decimal? Discount { get; set; }

        [Range(0, 100, ErrorMessage = "L'IVA deve essere tra 0 e 100")]
        public decimal VatRate { get; set; } = 22.00m;

        [Required(ErrorMessage = "Il totale è obbligatorio")]
        public decimal TotalAmount { get; set; }

        // Pagamento
        [Required(ErrorMessage = "Il tipo di pagamento è obbligatorio")]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Da Pagare";

        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount { get; set; }
        public int? InstallmentsCount { get; set; }
        public decimal? InstallmentAmount { get; set; }

        // Stato vendita
        [StringLength(50)]
        public string SaleStatus { get; set; } = "Bozza";

        [StringLength(20)]
        public string SaleStatusCode { get; set; } = "DRAFT";

        // Documenti
        public int? InvoiceId { get; set; }

        [StringLength(50)]
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int? ReceiptId { get; set; }

        [StringLength(50)]
        public string? ReceiptNumber { get; set; }
        public DateTime? ReceiptDate { get; set; }

        // Venditore
        [StringLength(50)]
        public string? SellerCode { get; set; }

        [StringLength(100)]
        public string? SellerName { get; set; }

        // Note e accessori
        [StringLength(2000)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? IncludedAccessories { get; set; }

        // Garanzia
        public bool HasWarranty { get; set; } = false;
        public int? WarrantyMonths { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }

        // Date
        public DateTime? SaleDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Metadata
        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}