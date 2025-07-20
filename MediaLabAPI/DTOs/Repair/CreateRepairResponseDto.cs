namespace MediaLabAPI.DTOs
{
    public class CreateRepairResponseDto
    {
        public int RepairId { get; set; }                     // ID numerico interno
        public Guid RepairGuid { get; set; }                  // 🆕 GUID univoco
        public string RepairCode { get; set; } = string.Empty; // 🆕 Codice ricercabile
        public Guid? CustomerId { get; set; }
        public int? DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool HasIncomingTest { get; set; }             // 🆕 Indica se ha diagnostica
        public string? IncomingTestSummary { get; set; }      // 🆕 Riassunto diagnostica
    }
}