namespace MediaLabAPI.DTOs
{
    public class UpdateRepairRequestDto
    {
        public Guid? CustomerId { get; set; }                // Cliente associato (opzionale)
        public Guid? DeviceId { get; set; }                  // Dispositivo associato (opzionale)
        public RepairDataDto RepairData { get; set; } = new();
        public string? Notes { get; set; }
        public List<DiagnosticItemDto>? DiagnosticItems { get; set; }
    }
}
