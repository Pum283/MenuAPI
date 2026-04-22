namespace Application.DTOs.Menu
{
    public class UpsertMenuCategoryRequest
    {
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

