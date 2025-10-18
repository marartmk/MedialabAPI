namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class DeviceInventoryDto
    {
        public int Id { get; set; }
        public Guid DeviceId { get; set; }
        public string Code { get; set; } = string.Empty;

        // Tipo e specifiche
        public string DeviceType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        // Identificativi
        public string IMEI { get; set; } = string.Empty;
        public string? ESN { get; set; }
        public string? SerialNumber { get; set; }
        public string Color { get; set; } = string.Empty;

        // Condizione e status
        public string DeviceCondition { get; set; } = string.Empty;
        public bool IsCourtesyDevice { get; set; }
        public string DeviceStatus { get; set; } = string.Empty;

        // Dati commerciali
        public string SupplierId { get; set; } = string.Empty;
        public string? SupplierName { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime? PurchaseDate { get; set; }

        // Ubicazione e note
        public string? Location { get; set; }
        public string? Notes { get; set; }

        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}