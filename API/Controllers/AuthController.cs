using Application.DTOs.Auth;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly AppDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(AppDbContext db, IJwtTokenService jwtTokenService)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _db.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);

            if (user == null)
            {
                return BadRequest(new { message = "Email hoặc mật khẩu không đúng." });
            }

            if (user.Status == UserStatus.Blocked)
            {
                return BadRequest(new { message = "Tài khoản đã bị chặn." });
            }

            var ok = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!ok)
            {
                return BadRequest(new { message = "Email hoặc mật khẩu không đúng." });
            }

            var roles = user.UserRoles.Select(x => x.Role.Code.ToString().ToLowerInvariant()).ToList();
            var token = _jwtTokenService.CreateAccessToken(user, roles);

            return Ok(new LoginResponse
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }
    }
}

