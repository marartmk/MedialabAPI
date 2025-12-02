using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    /// <summary>
    /// Modello per gli acquisti di apparati (nuovi e usati)
    /// </summary>
    [Table("DevicePurchases")]
    public class DevicePurchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid PurchaseId { get; set; } = Guid.NewGuid();

        [MaxLength(50)]
        public string? PurchaseCode { get; set; }

        // ==================== TIPO E CONDIZIONE ====================
        [Required]
        [MaxLength(20)]
        public string PurchaseType { get; set; } = "Apparato";

        [Required]
        [MaxLength(20)]
        public string DeviceCondition { get; set; } = "Nuovo"; // Nuovo, Usato

        // ==================== IDs PRODOTTO ====================
        public Guid? DeviceId { get; set; }
        public int? DeviceRegistryId { get; set; }
        public int? AccessoryId { get; set; }

        // ==================== DATI PRODOTTO ====================
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? IMEI { get; set; }

        // ==================== IDs RELAZIONALI ====================
        [Required]
        public Guid SupplierId { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        // ==================== PREZZI ====================
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OtherCosts { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal VatRate { get; set; } = 22.00m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // ==================== PAGAMENTO ====================
        [Required]
        [MaxLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Da Pagare";

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        public int? InstallmentsCount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? InstallmentAmount { get; set; }

        // ==================== STATO ACQUISTO ====================
        [Required]
        [MaxLength(50)]
        public string PurchaseStatus { get; set; } = "Bozza";

        [Required]
        [MaxLength(20)]
        public string PurchaseStatusCode { get; set; } = "DRAFT";

        // ==================== DOCUMENTI FORNITORE ====================
        public int? SupplierInvoiceId { get; set; }

        [MaxLength(50)]
        public string? SupplierInvoiceNumber { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? SupplierInvoiceDate { get; set; }

        [MaxLength(50)]
        public string? OrderNumber { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? OrderDate { get; set; }

        // ==================== DDT ====================
        [MaxLength(50)]
        public string? DDTNumber { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DDTDate { get; set; }

        // ==================== ACQUIRENTE/RESPONSABILE ====================
        [MaxLength(50)]
        public string? BuyerCode { get; set; }

        [MaxLength(100)]
        public string? BuyerName { get; set; }

        // ==================== NOTE E ACCESSORI ====================
        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? IncludedAccessories { get; set; }

        // ==================== CONTROLLO QUALITÀ ====================
        [MaxLength(50)]
        public string? QualityCheckStatus { get; set; } // Da Verificare, Verificato OK, Verificato con Difetti, Non Funzionante

        [MaxLength(1000)]
        public string? QualityCheckNotes { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? QualityCheckDate { get; set; }

        [MaxLength(100)]
        public string? QualityCheckedBy { get; set; }

        // ==================== GARANZIA FORNITORE ====================
        [Required]
        public bool HasSupplierWarranty { get; set; } = false;

        public int? SupplierWarrantyMonths { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? SupplierWarrantyExpiryDate { get; set; }

        // ==================== DATE ====================
        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime2")]
        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ReceivedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedAt { get; set; }

        // ==================== METADATA ====================
        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        // ==================== SOFT DELETE ====================
        [Required]
        public bool IsDeleted { get; set; } = false;

        [Column(TypeName = "datetime2")]
        public DateTime? DeletedAt { get; set; }

        [MaxLength(100)]
        public string? DeletedBy { get; set; }

        // ==================== NAVIGATION PROPERTIES ====================
        [ForeignKey("DeviceRegistryId")]
        public virtual DeviceRegistry? Device { get; set; }

        [ForeignKey("SupplierId")]
        public virtual C_ANA_Company? Supplier { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }

        public virtual ICollection<PurchasePayment>? Payments { get; set; }
    }
}