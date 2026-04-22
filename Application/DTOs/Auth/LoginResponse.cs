namespace Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}

