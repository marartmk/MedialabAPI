namespace MediaLabAPI.DTOs.Common
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public Guid IdCompany { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsEnabled { get; set; }
        public string? AccessLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public Guid? IdWhr { get; set; }
    }
}