using Domain.Common;

namespace Domain.Entity
{
    public class MenuItem : BaseAuditableEntity, ISoftDelete
    {
        public int CafeId { get; set; }
        public Cafe Cafe { get; set; } = null!;

        public int CategoryId { get; set; }
        public MenuCategory Category { get; set; } = null!;

        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }

        public decimal? BasePrice { get; set; }
        public string Currency { get; set; } = "VND";

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
}

