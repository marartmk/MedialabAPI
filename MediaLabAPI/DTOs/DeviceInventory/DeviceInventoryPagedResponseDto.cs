namespace MediaLabAPI.DTOs.DeviceInventory;

public class DeviceInventoryPagedResponseDto
{
    public List<DeviceInventoryDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
    public DeviceInventoryStatsDto? Stats { get; set; }
}