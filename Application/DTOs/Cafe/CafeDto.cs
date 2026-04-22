namespace Application.DTOs.Cafe
{
    public class CafeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }
        public string? HeroImageUrl { get; set; }

        public bool IsActive { get; set; }
    }
}

