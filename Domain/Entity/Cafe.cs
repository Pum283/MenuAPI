using Domain.Common;

namespace Domain.Entity
{
    public class Cafe : BaseAuditableEntity, ISoftDelete
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? LogoUrl { get; set; }
        public string? LogoPublicId { get; set; }

        public string? HeroImageUrl { get; set; }
        public string? HeroImagePublicId { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<CafeMember> Members { get; set; } = new List<CafeMember>();
        public ICollection<MenuCategory> Categories { get; set; } = new List<MenuCategory>();
        public ICollection<CafeTranslation> Translations { get; set; } = new List<CafeTranslation>();
    }
}

