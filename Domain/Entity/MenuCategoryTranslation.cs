using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class MenuCategoryTranslation : BaseEntity, IAuditableEntity
    {
        public Guid MenuCategoryId { get; set; }
        public MenuCategory MenuCategory { get; set; } = null!;

        public LanguageCode Language { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

