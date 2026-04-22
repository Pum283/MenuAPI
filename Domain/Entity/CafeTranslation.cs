using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class CafeTranslation : BaseEntity, IAuditableEntity
    {
        public Guid CafeId { get; set; }
        public Cafe Cafe { get; set; } = null!;

        public LanguageCode Language { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

