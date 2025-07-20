namespace MediaLabAPI.DTOs
{
    public class UpdateRepairStatusDto
    {
        public string StatusCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}