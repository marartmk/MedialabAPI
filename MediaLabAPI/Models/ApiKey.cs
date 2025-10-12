namespace MediaLabAPI.Models
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ApiKeyValue { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
