namespace MediaLabAPI.DTOs.Common
{
    public class UsersListDto
    {
        public List<UserDetailDto> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}