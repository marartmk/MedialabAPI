using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    /// <summary>
    /// Modello per i pagamenti degli acquisti
    /// </summary>
    [Table("PurchasePayments")]
    public class PurchasePayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public Guid PaymentId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PurchaseId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? TransactionReference { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? PaidBy { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ==================== NAVIGATION PROPERTIES ====================
        [ForeignKey("PurchaseId")]
        public virtual DevicePurchase? Purchase { get; set; }
    }
}