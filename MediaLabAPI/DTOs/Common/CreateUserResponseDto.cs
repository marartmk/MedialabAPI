namespace MediaLabAPI.DTOs.Common
{
    public class CreateUserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid IdCompany { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAdmin { get; set; }
        public string? AccessLevel { get; set; }
    }
}