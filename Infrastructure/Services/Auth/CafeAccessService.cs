using Application.Common.Interfaces;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Auth
{
    public class CafeAccessService : ICafeAccessService
    {
        private readonly AppDbContext _db;

        public CafeAccessService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<bool> IsAdminAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _db.UserRoles
                .AnyAsync(x => x.UserId == userId && x.Role.Code == RoleCode.Admin, cancellationToken);
        }

        public async Task<bool> CanManageCafeAsync(Guid userId, Guid cafeId, CancellationToken cancellationToken = default)
        {
            if (await IsAdminAsync(userId, cancellationToken)) return true;

            // Brand: needs active membership
            return await _db.CafeMembers
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.CafeId == cafeId &&
                    x.Status == MemberStatus.Active &&
                    x.User.Status == UserStatus.Active &&
                    x.Cafe.IsActive,
                    cancellationToken);
        }
    }
}

