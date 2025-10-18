using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class DeviceInventory
    {
        public int Id { get; set; }

        [Required]
        public Guid DeviceId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        // Tipo e specifiche
        [Required]
        [MaxLength(20)]
        public string DeviceType { get; set; } = "smartphone"; // smartphone | tablet

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        // Identificativi
        [Required]
        [MaxLength(50)]
        public string IMEI { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ESN { get; set; }

        [MaxLength(100)]
        public string? SerialNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

        // Condizione e status
        [Required]
        [MaxLength(20)]
        public string DeviceCondition { get; set; } = "new"; // new | used | refurbished

        public bool IsCourtesyDevice { get; set; } = false;

        [Required]
        [MaxLength(20)]
        public string DeviceStatus { get; set; } = "available"; // available | loaned | sold | unavailable

        // Dati commerciali
        [Required]
        [MaxLength(50)]
        public string SupplierId { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal SellingPrice { get; set; } = 0;

        public DateTime? PurchaseDate { get; set; }

        // Ubicazione e note
        [MaxLength(100)]
        public string? Location { get; set; }

        public string? Notes { get; set; }

        // Multitenant
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public virtual C_ANA_Company? Company { get; set; }
        public virtual DeviceInventorySupplier? Supplier { get; set; }
    }
}