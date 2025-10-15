namespace MediaLabAPI.DTOs
{
    // DTO per creare un nuovo pagamento
    public class CreateRepairPaymentDto
    {
        public Guid RepairId { get; set; }
        public Guid MultitenantId { get; set; }
        public decimal PartsAmount { get; set; } = 0;
        public decimal LaborAmount { get; set; } = 0;
        public string? Notes { get; set; }
    }

    // DTO per aggiornare un pagamento esistente
    public class UpdateRepairPaymentDto
    {
        public Guid MultitenantId { get; set; }
        public decimal PartsAmount { get; set; }
        public decimal LaborAmount { get; set; }
        public string? Notes { get; set; }
    }

    // DTO per la risposta completa con dati calcolati
    public class RepairPaymentResponseDto
    {
        public int Id { get; set; }
        public Guid RepairId { get; set; }
        public string? RepairCode { get; set; }
        public decimal PartsAmount { get; set; }
        public decimal LaborAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid CompanyId { get; set; }
        public Guid MultitenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; }
    }

    // DTO per il riepilogo pagamento (usato in liste)
    public class RepairPaymentSummaryDto
    {
        public int Id { get; set; }
        public Guid RepairId { get; set; }
        public string? RepairCode { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}