using System.ComponentModel.DataAnnotations;

namespace MediaLabAPI.DTOs
{
    // DTO per la risposta paginata
    public class WarehouseItemsPagedResponseDto
    {
        public List<WarehouseItemDetailDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        // Statistiche
        public WarehouseStatsDto Stats { get; set; } = new();
    }
}