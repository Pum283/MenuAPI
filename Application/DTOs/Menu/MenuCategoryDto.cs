namespace Application.DTOs.Menu
{
    public class MenuCategoryDto
    {
        public Guid Id { get; set; }
        public Guid CafeId { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}

