namespace MediaLabAPI.DTOs
{
    public class DeviceDataDto
    {
        public string SerialNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string DeviceType { get; set; } = "Mobile";
        public string? Color { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? Retailer { get; set; }
        public string? Notes { get; set; }
    }
}