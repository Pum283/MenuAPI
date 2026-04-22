using Application.DTOs.Common;

namespace Application.DTOs.Menu
{
    public class UpsertMenuCategoryRequest
    {
        public string Slug { get; set; } = string.Empty;
        public CategoryTranslationsRequest Translations { get; set; } = new();
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

