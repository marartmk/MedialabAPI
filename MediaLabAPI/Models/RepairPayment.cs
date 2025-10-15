using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("RepairPayments")]
    public class RepairPayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid RepairId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal PartsAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal LaborAmount { get; set; } = 0;

        // Campi calcolati dal database (PERSISTED)
        [Column(TypeName = "decimal(12,2)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? VatAmount { get; private set; }

        [Column(TypeName = "decimal(12,2)")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal? TotalAmount { get; private set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        [StringLength(100)]
        public string? DeletedBy { get; set; }

        // Navigation Properties
        [ForeignKey("RepairId")]
        public virtual DeviceRepair? DeviceRepair { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }
    }
}