using System;
using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs.Purchase
{
    /// <summary>
    /// DTO per la creazione di un nuovo acquisto
    /// </summary>
    public class CreatePurchaseRequestDto
    {
        // ==================== TIPO E CONDIZIONE ====================
        [Required(ErrorMessage = "Il tipo acquisto è obbligatorio")]
        [StringLength(20)]
        public string PurchaseType { get; set; } = "Apparato";

        [Required(ErrorMessage = "La condizione dell'apparato è obbligatoria")]
        [StringLength(20)]
        public string DeviceCondition { get; set; } = "Nuovo"; // Nuovo, Usato

        // ==================== IDs PRODOTTO ====================
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // ==================== DATI PRODOTTO ====================
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

        // ==================== IDs RELAZIONALI ====================
        [Required(ErrorMessage = "Il SupplierId è obbligatorio")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "Il CompanyId è obbligatorio")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "Il MultitenantId è obbligatorio")]
        public Guid MultitenantId { get; set; }

        // ==================== PREZZI ====================
        [Required(ErrorMessage = "Il prezzo di acquisto è obbligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il prezzo deve essere maggiore di 0")]
        public decimal PurchasePrice { get; set; }

        public decimal? ShippingCost { get; set; }
        public decimal? OtherCosts { get; set; }

        [Range(0, 100, ErrorMessage = "L'IVA deve essere tra 0 e 100")]
        public decimal VatRate { get; set; } = 22.00m;

        [Required(ErrorMessage = "Il totale è obbligatorio")]
        public decimal TotalAmount { get; set; }

        // ==================== PAGAMENTO ====================
        [Required(ErrorMessage = "Il tipo di pagamento è obbligatorio")]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Da Pagare";

        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount { get; set; }
        public int? InstallmentsCount { get; set; }
        public decimal? InstallmentAmount { get; set; }

        // ==================== STATO ACQUISTO ====================
        [StringLength(50)]
        public string PurchaseStatus { get; set; } = "Bozza";

        [StringLength(20)]
        public string PurchaseStatusCode { get; set; } = "DRAFT";

        // ==================== DOCUMENTI FORNITORE ====================
        public int? SupplierInvoiceId { get; set; }

        [StringLength(50)]
        public string? SupplierInvoiceNumber { get; set; }
        public DateTime? SupplierInvoiceDate { get; set; }

        [StringLength(50)]
        public string? OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }

        // ==================== DDT ====================
        [StringLength(50)]
        public string? DDTNumber { get; set; }
        public DateTime? DDTDate { get; set; }

        // ==================== ACQUIRENTE/RESPONSABILE ====================
        [StringLength(50)]
        public string? BuyerCode { get; set; }

        [StringLength(100)]
        public string? BuyerName { get; set; }

        // ==================== NOTE E ACCESSORI ====================
        [StringLength(2000)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? IncludedAccessories { get; set; }

        // ==================== CONTROLLO QUALITÀ ====================
        [StringLength(50)]
        public string? QualityCheckStatus { get; set; }

        [StringLength(1000)]
        public string? QualityCheckNotes { get; set; }
        public DateTime? QualityCheckDate { get; set; }

        [StringLength(100)]
        public string? QualityCheckedBy { get; set; }

        // ==================== GARANZIA FORNITORE ====================
        public bool HasSupplierWarranty { get; set; } = false;
        public int? SupplierWarrantyMonths { get; set; }
        public DateTime? SupplierWarrantyExpiryDate { get; set; }

        // ==================== DATE ====================
        public DateTime? PurchaseDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        // ==================== METADATA ====================
        [StringLength(100)]
        public string? CreatedBy { get; set; }
    }
}
