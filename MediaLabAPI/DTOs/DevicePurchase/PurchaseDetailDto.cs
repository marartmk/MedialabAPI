using System;
using System.Collections.Generic;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO con i dettagli completi di un acquisto
    /// </summary>
    public class PurchaseDetailDto
    {
        public int Id { get; set; }
        public Guid PurchaseId { get; set; }
        public string? PurchaseCode { get; set; }

        // ==================== TIPO E CONDIZIONE ====================
        public string PurchaseType { get; set; } = "Apparato";
        public string DeviceCondition { get; set; } = "Nuovo";

        // ==================== IDs PRODOTTO ====================
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // ==================== DATI PRODOTTO ====================
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public string? IMEI { get; set; }

        // ==================== IDs RELAZIONALI ====================
        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid MultitenantId { get; set; }

        // ==================== PREZZI ====================
        public decimal PurchasePrice { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? OtherCosts { get; set; }
        public decimal VatRate { get; set; }
        public decimal TotalAmount { get; set; }

        // ==================== PAGAMENTO ====================
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Da Pagare";
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int? InstallmentsCount { get; set; }
        public decimal? InstallmentAmount { get; set; }

        // ==================== STATO ACQUISTO ====================
        public string PurchaseStatus { get; set; } = "Bozza";
        public string PurchaseStatusCode { get; set; } = "DRAFT";

        // ==================== DOCUMENTI FORNITORE ====================
        public int? SupplierInvoiceId { get; set; }
        public string? SupplierInvoiceNumber { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }

        // ==================== DDT ====================
        public string? DDTNumber { get; set; }
        public DateTime? DDTDate { get; set; }

        // ==================== ACQUIRENTE/RESPONSABILE ====================
        public string? BuyerCode { get; set; }
        public string? BuyerName { get; set; }

        // ==================== NOTE E ACCESSORI ====================
        public string? Notes { get; set; }
        public string? IncludedAccessories { get; set; }

        // ==================== CONTROLLO QUALITÀ ====================
        public string? QualityCheckStatus { get; set; }
        public string? QualityCheckNotes { get; set; }
        public DateTime? QualityCheckDate { get; set; }
        public string? QualityCheckedBy { get; set; }

        // ==================== GARANZIA FORNITORE ====================
        public bool HasSupplierWarranty { get; set; }
        public int? SupplierWarrantyMonths { get; set; }
        public DateTime? SupplierWarrantyExpiryDate { get; set; }

        // ==================== DATE ====================
        public DateTime CreatedAt { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // ==================== METADATA ====================
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // ==================== LISTA PAGAMENTI ====================
        public List<PurchasePaymentDetailDto>? Payments { get; set; }
    }
}
