using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.Models
{
    public class WarehouseItem
    {
        public int Id { get; set; }

        [Required]
        public Guid ItemId { get; set; } // GUID univoco per l'articolo

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // Codice articolo (es: SCR-IP14-BLK)

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // Nome articolo

        [MaxLength(500)]
        public string? Description { get; set; } // Descrizione dettagliata

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty; // Categoria (screens, batteries, etc.)

        [MaxLength(100)]
        public string? Subcategory { get; set; } // Sottocategoria

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty; // Marca

        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty; // Modello

        [Required]
        [MaxLength(50)]
        public string Supplier { get; set; } = string.Empty; // ID Fornitore

        public int Quantity { get; set; } // Quantità disponibile

        public int MinQuantity { get; set; } // Quantità minima

        public int MaxQuantity { get; set; } // Quantità massima

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; } // Prezzo unitario

        [Range(0, double.MaxValue)]
        public decimal TotalValue { get; set; } // Valore totale (calcolato)

        [MaxLength(100)]
        public string? Location { get; set; } // Ubicazione magazzino (es: A-01-015)

        [MaxLength(1000)]
        public string? Notes { get; set; } // Note aggiuntive

        // Campi standard per multitenant e company
        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid MultitenantId { get; set; }

        // Campi di auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual C_ANA_Company? Company { get; set; }
    }   
}