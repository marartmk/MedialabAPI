namespace MediaLabAPI.DTOs
{
    public class RepairDataDto
    {
        public string FaultDeclared { get; set; } = string.Empty;
        public string? RepairAction { get; set; }
        public string? TechnicianCode { get; set; }
        public string? TechnicianName { get; set; }
        public decimal? EstimatedPrice { get; set; }
        public string? PaymentType { get; set; }
        public string? BillingInfo { get; set; }
        public string? UnlockCode { get; set; }
        public string? CourtesyPhone { get; set; }
    }
}