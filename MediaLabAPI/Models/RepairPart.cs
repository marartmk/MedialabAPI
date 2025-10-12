using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("RepairParts")]
    public class RepairPart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Collegamenti
        [Required]
        public Guid RepairId { get; set; }

        [Required]
        public int WarehouseItemId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        // Snapshot dati articolo (al momento dell'uso)
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        // Quantità e prezzi
        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal LineTotal { get; set; } = 0;

        // Metadati
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(RepairId))]
        public virtual DeviceRepair? DeviceRepair { get; set; }

        [ForeignKey(nameof(WarehouseItemId))]
        public virtual WarehouseItem? WarehouseItem { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public virtual C_ANA_Company? Company { get; set; }
    }
}