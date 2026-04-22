using Domain.Common;

namespace Domain.Entity
{
    public class MenuCategory : BaseAuditableEntity, ISoftDelete
    {
        public int CafeId { get; set; }
        public Cafe Cafe { get; set; } = null!;

        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}

