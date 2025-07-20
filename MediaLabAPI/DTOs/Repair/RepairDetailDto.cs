namespace MediaLabAPI.DTOs
{
    // 📋 DTO per dettaglio completo riparazione
    public class RepairDetailDto
    {
        // Identificatori
        public int Id { get; set; }
        public Guid RepairId { get; set; }
        public string RepairCode { get; set; } = string.Empty;

        // Dettagli Cliente
        public CustomerDetailDto Customer { get; set; } = new();

        // Dettagli Dispositivo  
        public DeviceDetailDto Device { get; set; } = new();

        // Dati Riparazione
        public string FaultDeclared { get; set; } = string.Empty;
        public string? RepairAction { get; set; }
        public string RepairStatus { get; set; } = string.Empty;
        public string RepairStatusCode { get; set; } = string.Empty;
        public string? TechnicianCode { get; set; }
        public string? TechnicianName { get; set; }
        public string? Notes { get; set; }

        // Timeline Date
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

        // Diagnostica
        public bool HasDiagnostic { get; set; }
        public string? DiagnosticSummary { get; set; }
        public IncomingTestDto? DiagnosticDetails { get; set; }  // Dettaglio completo diagnostica se necessario
    }

    // 👤 DTO per dettagli cliente
    public class CustomerDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? Region { get; set; }
        public string? FiscalCode { get; set; }
        public string? VatNumber { get; set; }
        public string CustomerType { get; set; } = string.Empty; // "Privato" o "Azienda"
    }

    // 📱 DTO per dettagli dispositivo
    public class DeviceDetailDto
    {
        public Guid DeviceId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public string? DeviceType { get; set; }
        public string? Color { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? Retailer { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}