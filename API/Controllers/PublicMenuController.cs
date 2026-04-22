using Infrastructure.Persistence;
using API.Common.Language;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Enum;

namespace API.Controllers
{
    public class PublicMenuController : ApiController
    {
        private readonly AppDbContext _db;

        public PublicMenuController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{cafeCode}/menu")]
        public async Task<IActionResult> GetMenuByCafeCode(string cafeCode, CancellationToken cancellationToken)
        {
            var code = cafeCode.Trim().ToLowerInvariant();
            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;

            var cafe = await _db.Cafes.FirstOrDefaultAsync(x => x.Code == code && x.IsActive, cancellationToken);
            if (cafe == null) return NotFound();

            var categories = await _db.MenuCategories
                .Where(x => x.CafeId == cafe.Id && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.Id,
                    x.Slug,
                    Name =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Name,
                    Description =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Description,
                    x.DisplayOrder,
                    items = x.Items
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new
                        {
                            i.Id,
                            i.Slug,
                            Name =
                                i.Translations.Where(t => t.Language == lang).Select(t => t.Name).FirstOrDefault()
                                ?? i.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Name).FirstOrDefault()
                                ?? i.Name,
                            Description =
                                i.Translations.Where(t => t.Language == lang).Select(t => t.Description).FirstOrDefault()
                                ?? i.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Description).FirstOrDefault()
                                ?? i.Description,
                            i.ImageUrl,
                            i.BasePrice,
                            i.Currency,
                            i.DisplayOrder
                        })
                })
                .ToListAsync(cancellationToken);

            var cafeName = await _db.CafeTranslations
                .Where(t => t.CafeId == cafe.Id && (t.Language == lang || t.Language == fallbackLang))
                .OrderBy(t => t.Language == lang ? 0 : 1)
                .Select(t => t.Name)
                .FirstOrDefaultAsync(cancellationToken) ?? cafe.Name;

            return Ok(new
            {
                cafe = new
                {
                    cafe.Id,
                    cafe.Code,
                    Name = cafeName,
                    cafe.LogoUrl,
                    cafe.HeroImageUrl
                },
                lang = lang.ToString().ToLowerInvariant(),
                categories
            });
        }
    }
}

