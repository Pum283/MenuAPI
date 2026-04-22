namespace Infrastructure.Services.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = "MenuAPI";
        public string Audience { get; set; } = "MenuAPI";
        public string SigningKey { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 720;
    }
}

