namespace Application.DTOs.Menu
{
    public class UpsertMenuItemRequest
    {
        public int CategoryId { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? BasePrice { get; set; }
        public string Currency { get; set; } = "VND";
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

