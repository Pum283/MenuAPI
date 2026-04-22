namespace Application.DTOs.Menu
{
    public class MenuCategoryDto
    {
        public int Id { get; set; }
        public int CafeId { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

