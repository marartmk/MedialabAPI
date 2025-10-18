namespace MediaLabAPI.DTOs.DeviceInventory;

public class DeviceInventoryStatsDto
{
    public int TotalDevices { get; set; }
    public int AvailableDevices { get; set; }
    public int LoanedDevices { get; set; }
    public int SoldDevices { get; set; }
    public int CourtesyDevices { get; set; }
    public int Smartphones { get; set; }
    public int Tablets { get; set; }

    public decimal TotalPurchaseValue { get; set; }
    public decimal TotalSellingValue { get; set; }
    public decimal PotentialProfit { get; set; }

    public int NewDevices { get; set; }
    public int UsedDevices { get; set; }
    public int RefurbishedDevices { get; set; }
}