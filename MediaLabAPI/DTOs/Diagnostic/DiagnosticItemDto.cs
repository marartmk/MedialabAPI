namespace MediaLabAPI.DTOs
{
    public class DiagnosticItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool Active { get; set; }
    }
}