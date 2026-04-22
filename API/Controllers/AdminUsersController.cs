using Application.DTOs.Auth;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminUsersController : ApiController
    {
        private readonly AppDbContext _db;

        public AdminUsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? q, CancellationToken cancellationToken)
        {
            var query = _db.Users.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLowerInvariant();
                query = query.Where(x => x.Email.ToLower().Contains(keyword));
            }

            var users = await query
                .OrderBy(x => x.Email)
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Status = u.Status.ToString().ToLowerInvariant(),
                    Roles = u.UserRoles.Select(ur => ur.Role.Code.ToString().ToLowerInvariant()).ToList(),
                    CafeMemberships = u.CafeMembers
                        .OrderBy(cm => cm.Cafe.Name)
                        .Select(cm => new AdminUserCafeMembershipDto
                        {
                            CafeId = cm.CafeId,
                            CafeCode = cm.Cafe.Code,
                            CafeName = cm.Cafe.Name,
                            Status = cm.Status.ToString().ToLowerInvariant()
                        })
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Email và mật khẩu là bắt buộc." });
            }

            var exists = await _db.Users.AnyAsync(x => x.Email.ToLower() == email, cancellationToken);
            if (exists)
            {
                return BadRequest(new { message = "Email đã tồn tại." });
            }

            var roleCode = request.RoleCode.Trim().ToLowerInvariant();
            var targetRole = roleCode switch
            {
                "admin" => RoleCode.Admin,
                "brand" => RoleCode.Brand,
                _ => (RoleCode?)null
            };
            if (targetRole == null)
            {
                return BadRequest(new { message = "RoleCode phải là admin hoặc brand." });
            }

            var role = await _db.Roles.FirstAsync(r => r.Code == targetRole.Value, cancellationToken);

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = UserStatus.Active
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new { user.Id, user.Email, Role = role.Code.ToString().ToLowerInvariant() });
        }

        [HttpPut("{userId:guid}/block")]
        public async Task<IActionResult> BlockUser(Guid userId, [FromBody] BlockUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null) return NotFound();

            if (request.IsBlocked)
            {
                user.Status = UserStatus.Blocked;
                user.BlockedAt = DateTime.UtcNow;
                user.BlockedReason = request.Reason;
            }
            else
            {
                user.Status = UserStatus.Active;
                user.BlockedAt = null;
                user.BlockedReason = null;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { user.Id, user.Status });
        }

        [HttpPut("memberships")]
        public async Task<IActionResult> UpsertCafeMember([FromBody] AssignCafeMemberRequest request, CancellationToken cancellationToken)
        {
            var cafeExists = await _db.Cafes.AnyAsync(x => x.Id == request.CafeId, cancellationToken);
            if (!cafeExists) return BadRequest(new { message = "CafeId không tồn tại." });

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
            if (user == null) return BadRequest(new { message = "UserId không tồn tại." });

            var member = await _db.CafeMembers
                .FirstOrDefaultAsync(x => x.CafeId == request.CafeId && x.UserId == request.UserId, cancellationToken);

            if (member == null)
            {
                member = new CafeMember
                {
                    CafeId = request.CafeId,
                    UserId = request.UserId,
                };
                _db.CafeMembers.Add(member);
            }

            if (request.IsBlocked)
            {
                member.Status = MemberStatus.Blocked;
                member.BlockedAt = DateTime.UtcNow;
                member.BlockedReason = request.Reason;
            }
            else
            {
                member.Status = MemberStatus.Active;
                member.BlockedAt = null;
                member.BlockedReason = null;
            }

            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { member.Id, member.CafeId, member.UserId, member.Status });
        }
    }
}

