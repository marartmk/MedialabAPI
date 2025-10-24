using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("DeviceSales")]
    public class DeviceSale
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// GUID univoco per ogni vendita
        /// </summary>
        [Required]
        public Guid SaleId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Codice vendita ricercabile (es: SALE-2024-00001)
        /// </summary>
        [StringLength(50)]
        public string? SaleCode { get; set; }

        /// <summary>
        /// Tipo vendita: "Apparato" o "Accessorio"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string SaleType { get; set; } = "Apparato";

        /// <summary>
        /// ID del dispositivo (GUID per logica applicativa)
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// ID numerico del dispositivo da DeviceRegistry (FK al database)
        /// </summary>
        public int? DeviceRegistryId { get; set; }

        /// <summary>
        /// ID dell'accessorio (se vendita accessorio - da implementare)
        /// </summary>
        public int? AccessoryId { get; set; }

        /// <summary>
        /// Brand del prodotto venduto
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Modello del prodotto venduto
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Numero seriale (se applicabile)
        /// </summary>
        [StringLength(100)]
        public string? SerialNumber { get; set; }

        /// <summary>
        /// IMEI (se applicabile per telefoni)
        /// </summary>
        [StringLength(50)]
        public string? IMEI { get; set; }

        /// <summary>
        /// ID del cliente
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// ID dell'azienda
        /// </summary>
        [Required]
        public Guid CompanyId { get; set; }

        /// <summary>
        /// ID del tenant
        /// </summary>
        [Required]
        public Guid MultitenantId { get; set; }

        /// <summary>
        /// Prezzo di vendita
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Prezzo originale (per sconti)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        /// <summary>
        /// Sconto applicato
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }

        /// <summary>
        /// IVA applicata (%)
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        public decimal VatRate { get; set; } = 22.00M;

        /// <summary>
        /// Totale con IVA
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Tipo di pagamento: "Contanti", "Carta", "Bonifico", "Rate", etc.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        /// <summary>
        /// Stato pagamento: "Pagato", "Parziale", "Da Pagare"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Da Pagare";

        /// <summary>
        /// Importo già pagato (per acconti/rate)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        /// <summary>
        /// Importo rimanente da pagare
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingAmount { get; set; }

        /// <summary>
        /// Numero di rate (se pagamento rateale)
        /// </summary>
        public int? InstallmentsCount { get; set; }

        /// <summary>
        /// Importo rata
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InstallmentAmount { get; set; }

        /// <summary>
        /// Stato vendita: "Bozza", "Completata", "Annullata"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SaleStatus { get; set; } = "Bozza";

        /// <summary>
        /// Codice stato vendita
        /// </summary>
        [Required]
        [StringLength(20)]
        public string SaleStatusCode { get; set; } = "DRAFT";

        /// <summary>
        /// ID fattura (riferimento alla tabella fatture)
        /// </summary>
        public int? InvoiceId { get; set; }

        /// <summary>
        /// Numero fattura
        /// </summary>
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// Data fattura
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        /// ID ricevuta
        /// </summary>
        public int? ReceiptId { get; set; }

        /// <summary>
        /// Numero ricevuta
        /// </summary>
        [StringLength(50)]
        public string? ReceiptNumber { get; set; }

        /// <summary>
        /// Data ricevuta
        /// </summary>
        public DateTime? ReceiptDate { get; set; }

        /// <summary>
        /// Codice venditore
        /// </summary>
        [StringLength(50)]
        public string? SellerCode { get; set; }

        /// <summary>
        /// Nome venditore
        /// </summary>
        [StringLength(100)]
        public string? SellerName { get; set; }

        /// <summary>
        /// Note sulla vendita
        /// </summary>
        [StringLength(2000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Garanzia inclusa
        /// </summary>
        public bool HasWarranty { get; set; } = false;

        /// <summary>
        /// Durata garanzia (mesi)
        /// </summary>
        public int? WarrantyMonths { get; set; }

        /// <summary>
        /// Data scadenza garanzia
        /// </summary>
        public DateTime? WarrantyExpiryDate { get; set; }

        /// <summary>
        /// Accessori inclusi
        /// </summary>
        [StringLength(500)]
        public string? IncludedAccessories { get; set; }

        /// <summary>
        /// Data creazione
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data vendita effettiva
        /// </summary>
        public DateTime? SaleDate { get; set; }

        /// <summary>
        /// Data consegna
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Data ultimo aggiornamento
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Utente che ha creato la vendita
        /// </summary>
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Utente che ha aggiornato la vendita
        /// </summary>
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Soft delete
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Data cancellazione
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        [ForeignKey("DeviceRegistryId")]
        public virtual DeviceRegistry? Device { get; set; }

        [ForeignKey("CustomerId")]
        public virtual C_ANA_Company? Customer { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }

        // Relazione con i pagamenti
        public virtual ICollection<SalePayment>? Payments { get; set; }
    }

    /// <summary>
    /// Tabella per gestire i pagamenti rateali/acconti
    /// </summary>
    [Table("SalePayments")]
    public class SalePayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SaleId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // "Contanti", "Carta", "Bonifico"

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? ReceivedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        [ForeignKey("SaleId")]
        public virtual DeviceSale? Sale { get; set; }
    }
}