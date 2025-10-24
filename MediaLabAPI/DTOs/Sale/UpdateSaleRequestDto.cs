using System;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Sale
{
    /// <summary>
    /// DTO per l'aggiornamento di una vendita esistente
    /// </summary>
    public class UpdateSaleRequestDto
    {
        // Tipo vendita
        [StringLength(20)]
        public string? SaleType { get; set; }

        // IDs prodotto
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // Dati prodotto
        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(100)]
        public string? Model { get; set; }

        [StringLength(100)]
        public string? SerialNumber { get; set; }

        [StringLength(50)]
        public string? IMEI { get; set; }

        // IDs relazionali
        public Guid? CustomerId { get; set; }
        public Guid? CompanyId { get; set; }

        // Prezzi
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal? SalePrice { get; set; }

        public decimal? OriginalPrice { get; set; }
        public decimal? Discount { get; set; }

        [Range(0, 100, ErrorMessage = "L'IVA deve essere tra 0 e 100")]
        public decimal? VatRate { get; set; }

        public decimal? TotalAmount { get; set; }

        // Pagamento
        [StringLength(50)]
        public string? PaymentType { get; set; }

        [StringLength(50)]
        public string? PaymentStatus { get; set; }

        public decimal? PaidAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public int? InstallmentsCount { get; set; }
        public decimal? InstallmentAmount { get; set; }

        // Stato vendita
        [StringLength(50)]
        public string? SaleStatus { get; set; }

        [StringLength(20)]
        public string? SaleStatusCode { get; set; }

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
        public bool? HasWarranty { get; set; }
        public int? WarrantyMonths { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }

        // Date
        public DateTime? SaleDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        // Metadata
        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}