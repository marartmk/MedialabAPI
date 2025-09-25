using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la risposta dettagliata dell'articolo
    public class WarehouseItemDetailDto
    {
        public int Id { get; set; }
        public Guid ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Subcategory { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string? SupplierName { get; set; } // Nome del fornitore
        public int Quantity { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public string StockStatus { get; set; } = string.Empty; // available, low, out
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Informazioni categoria
        public CategoryInfoDto? CategoryInfo { get; set; }
    }
}