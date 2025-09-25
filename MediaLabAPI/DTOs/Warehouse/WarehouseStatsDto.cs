using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per le statistiche del magazzino
    public class WarehouseStatsDto
    {
        public int TotalItems { get; set; }
        public int AvailableItems { get; set; }
        public int LowStockItems { get; set; }
        public int OutOfStockItems { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
    }
}