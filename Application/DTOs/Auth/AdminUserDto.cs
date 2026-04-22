namespace Application.DTOs.Auth
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "active" | "blocked"
        public List<string> Roles { get; set; } = new();
        public List<AdminUserCafeMembershipDto> CafeMemberships { get; set; } = new();
    }

    public class AdminUserCafeMembershipDto
    {
        public Guid CafeId { get; set; }
        public string CafeCode { get; set; } = string.Empty;
        public string CafeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "active" | "blocked"
    }
}

