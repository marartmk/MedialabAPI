using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la lista semplificata (per dropdown e select)
    public class WarehouseItemLightDto
    {
        public int Id { get; set; }
        public Guid ItemId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}