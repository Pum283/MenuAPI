using Domain.Common;
using Domain.Enum;

namespace Domain.Entity
{
    public class CafeMember : BaseAuditableEntity
    {
        public int CafeId { get; set; }
        public Cafe Cafe { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public MemberStatus Status { get; set; } = MemberStatus.Active;
        public string? BlockedReason { get; set; }
        public DateTime? BlockedAt { get; set; }
    }
}

