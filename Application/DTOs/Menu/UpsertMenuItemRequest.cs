using Application.DTOs.Common;

namespace Application.DTOs.Menu
{
    public class UpsertMenuItemRequest
    {
        public Guid CategoryId { get; set; }
        public string Slug { get; set; } = string.Empty;
        public ItemTranslationsRequest Translations { get; set; } = new();
        public decimal? BasePrice { get; set; }
        public string Currency { get; set; } = "VND";
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

