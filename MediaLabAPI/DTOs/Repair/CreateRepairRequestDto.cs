namespace MediaLabAPI.DTOs
{
    public class CreateRepairRequestDto
    {
        // Dati Cliente (se esistente)
        public Guid? CustomerId { get; set; }

        // Dati Cliente (se nuovo)
        public CustomerDataDto? NewCustomer { get; set; }

        // Dati Dispositivo (se esistente)
        public int? DeviceId { get; set; }

        // Dati Dispositivo (se nuovo)
        public DeviceDataDto? NewDevice { get; set; }

        // Dati Riparazione
        public RepairDataDto RepairData { get; set; } = new();

        // 🆕 NUOVO: Diagnostica strutturata
        public IncomingTestDto? IncomingTest { get; set; }

        // 🔄 MANTENIAMO: Lista diagnostica per backward compatibility
        public List<DiagnosticItemDto> DiagnosticItems { get; set; } = new();

        // Informazioni aggiuntive
        public string? Notes { get; set; }
        public Guid MultitenantId { get; set; }
    }
}