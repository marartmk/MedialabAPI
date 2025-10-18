namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class DeviceInventorySupplierDto
    {
        public int Id { get; set; }
        public string SupplierId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int DeviceCount { get; set; }
    }
}