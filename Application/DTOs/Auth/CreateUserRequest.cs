namespace Application.DTOs.Auth
{
    public class CreateUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty; // "admin" | "brand"
    }
}

