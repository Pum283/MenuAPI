using Application.DTOs.Cafe;
using API.Common.Language;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminCafesController : ApiController
    {
        private readonly AppDbContext _db;

        public AdminCafesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;

            var cafes = await _db.Cafes
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
                .ToListAsync(cancellationToken);

            return Ok(cafes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UpsertCafeRequest request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToLowerInvariant();
            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
            {
                return BadRequest(new { message = "Code, Name (VI) và Name (EN) là bắt buộc." });
            }

            var exists = await _db.Cafes.AnyAsync(x => x.Code == code, cancellationToken);
            if (exists) return BadRequest(new { message = "Code đã tồn tại." });

            var cafe = new Cafe
            {
                Code = code,
                // keep legacy single-language column for compatibility (VI)
                Name = viName,
                IsActive = request.IsActive,
            };

            cafe.Translations.Add(new CafeTranslation { CafeId = cafe.Id, Language = LanguageCode.Vi, Name = viName });
            cafe.Translations.Add(new CafeTranslation { CafeId = cafe.Id, Language = LanguageCode.En, Name = enName });

            _db.Cafes.Add(cafe);
            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outName =
                cafe.Translations.FirstOrDefault(t => t.Language == lang)?.Name
                ?? cafe.Translations.FirstOrDefault(t => t.Language == fallbackLang)?.Name
                ?? cafe.Name;

            return Ok(new CafeDto
            {
                Id = cafe.Id,
                Code = cafe.Code,
                Name = outName,
                LogoUrl = cafe.LogoUrl,
                HeroImageUrl = cafe.HeroImageUrl,
                IsActive = cafe.IsActive
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpsertCafeRequest request, CancellationToken cancellationToken)
        {
            var cafe = await _db.Cafes
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (cafe == null) return NotFound();

            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
                return BadRequest(new { message = "Name (VI) và Name (EN) là bắt buộc." });

            cafe.Name = viName;
            cafe.IsActive = request.IsActive;

            UpsertCafeTranslation(cafe, LanguageCode.Vi, viName);
            UpsertCafeTranslation(cafe, LanguageCode.En, enName);

            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outName =
                cafe.Translations.FirstOrDefault(t => t.Language == lang)?.Name
                ?? cafe.Translations.FirstOrDefault(t => t.Language == fallbackLang)?.Name
                ?? cafe.Name;

            return Ok(new CafeDto
            {
                Id = cafe.Id,
                Code = cafe.Code,
                Name = outName,
                LogoUrl = cafe.LogoUrl,
                HeroImageUrl = cafe.HeroImageUrl,
                IsActive = cafe.IsActive
            });
        }

        private static void UpsertCafeTranslation(Cafe cafe, LanguageCode lang, string name)
        {
            var tr = cafe.Translations.FirstOrDefault(x => x.Language == lang);
            if (tr == null)
            {
                cafe.Translations.Add(new CafeTranslation { CafeId = cafe.Id, Language = lang, Name = name });
                return;
            }

            tr.Name = name;
        }
    }
}

