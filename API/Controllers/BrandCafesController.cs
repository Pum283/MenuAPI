using System.Security.Claims;
using API.Common.Language;
using Application.DTOs.Cafe;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    /// <summary>
    /// Brand: danh sách quán được gán cho user hiện tại.
    /// </summary>
    [Authorize(Roles = "brand")]
    public class BrandCafesController : ApiController
    {
        private readonly AppDbContext _db;

        public BrandCafesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCafes(CancellationToken cancellationToken)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(raw, out var userId))
            {
                return Unauthorized(new { message = "Invalid token." });
            }

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;

            var cafes = await _db.CafeMembers
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.Status == MemberStatus.Active &&
                    x.User.Status == UserStatus.Active &&
                    x.Cafe.IsActive)
                .Select(x => x.Cafe)
                .OrderBy(x => x.Id)
                .Select(x => new CafeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Name,
                    LogoUrl = x.LogoUrl,
                    HeroImageUrl = x.HeroImageUrl,
                    IsActive = x.IsActive
                })
                .Distinct()
                .ToListAsync(cancellationToken);

            return Ok(cafes);
        }
    }
}

