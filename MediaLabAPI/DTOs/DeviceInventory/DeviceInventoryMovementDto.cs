namespace MediaLabAPI.DTOs.DeviceInventory
{
    public class DeviceInventoryMovementDto
    {
        public int Id { get; set; }
        public Guid MovementId { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public string? FromStatus { get; set; }
        public string? ToStatus { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public DateTime MovementDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}