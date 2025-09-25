using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la ricerca nel magazzino
    public class WarehouseSearchDto
    {
        public string? SearchQuery { get; set; }
        public string? Category { get; set; }
        public string? Supplier { get; set; }
        public string? StockStatus { get; set; } // available, low, out
        public string? Brand { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Guid? MultitenantId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } // name, code, quantity, price, etc.
        public bool SortDescending { get; set; } = false;
    }
}