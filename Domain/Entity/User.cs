using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class User : BaseAuditableEntity, ISoftDelete
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public UserStatus Status { get; set; } = UserStatus.Active;
        public string? BlockedReason { get; set; }
        public DateTime? BlockedAt { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<CafeMember> CafeMembers { get; set; } = new List<CafeMember>();
    }
}

