using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class Role : BaseEntity
    {
        public RoleCode Code { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

