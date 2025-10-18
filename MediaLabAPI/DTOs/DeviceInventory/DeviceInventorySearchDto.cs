namespace MediaLabAPI.DTOs.DeviceInventory;

public class DeviceInventorySearchDto
{
    public string? SearchQuery { get; set; }
    public string? DeviceType { get; set; } // smartphone | tablet
    public string? Brand { get; set; }
    public string? DeviceCondition { get; set; } // new | used | refurbished
    public string? DeviceStatus { get; set; } // available | loaned | sold | unavailable
    public string? SupplierId { get; set; }
    public bool? IsCourtesyDevice { get; set; }

    public decimal? MinPurchasePrice { get; set; }
    public decimal? MaxPurchasePrice { get; set; }
    public decimal? MinSellingPrice { get; set; }
    public decimal? MaxSellingPrice { get; set; }

    public Guid? MultitenantId { get; set; }

    // Paginazione
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;

    // Ordinamento
    public string? SortBy { get; set; } = "created"; // code, brand, model, status, created
    public bool SortDescending { get; set; } = true;
}