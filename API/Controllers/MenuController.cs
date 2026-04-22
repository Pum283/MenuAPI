using Application.Common.Interfaces;
using Application.DTOs.Menu;
using API.Common.Language;
using Domain.Entity;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class MenuController : ApiController
    {
        private readonly AppDbContext _db;
        private readonly ICafeAccessService _access;

        public MenuController(AppDbContext db, ICafeAccessService access)
        {
            _db = db;
            _access = access;
        }

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }

        [HttpGet("{cafeId:guid}/categories")]
        public async Task<IActionResult> GetCategories(Guid cafeId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;

            var categories = await _db.MenuCategories
                .Where(x => x.CafeId == cafeId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new MenuCategoryDto
                {
                    Id = x.Id,
                    CafeId = x.CafeId,
                    Slug = x.Slug,
                    Name =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Name,
                    Description =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Description,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Ok(categories);
        }

        [HttpPost("{cafeId:guid}/categories")]
        public async Task<IActionResult> CreateCategory(Guid cafeId, [FromBody] UpsertMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var slug = request.Slug.Trim().ToLowerInvariant();
            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            var viDesc = request.Translations?.Vi?.Description;
            var enDesc = request.Translations?.En?.Description;
            if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
                return BadRequest(new { message = "Slug, Name (VI) và Name (EN) là bắt buộc." });

            var exists = await _db.MenuCategories.AnyAsync(x => x.CafeId == cafeId && x.Slug == slug, cancellationToken);
            if (exists) return BadRequest(new { message = "Slug đã tồn tại trong quán." });

            var category = new MenuCategory
            {
                CafeId = cafeId,
                Slug = slug,
                // legacy single-language columns (VI)
                Name = viName,
                Description = viDesc,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };

            category.Translations.Add(new MenuCategoryTranslation
            {
                MenuCategoryId = category.Id,
                Language = LanguageCode.Vi,
                Name = viName,
                Description = viDesc
            });
            category.Translations.Add(new MenuCategoryTranslation
            {
                MenuCategoryId = category.Id,
                Language = LanguageCode.En,
                Name = enName,
                Description = enDesc
            });

            _db.MenuCategories.Add(category);
            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outTr =
                category.Translations.FirstOrDefault(t => t.Language == lang)
                ?? category.Translations.FirstOrDefault(t => t.Language == fallbackLang);

            return Ok(new MenuCategoryDto
            {
                Id = category.Id,
                CafeId = category.CafeId,
                Slug = category.Slug,
                Name = outTr?.Name ?? category.Name,
                Description = outTr?.Description ?? category.Description,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            });
        }

        [HttpPut("{cafeId:guid}/categories/{categoryId:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid cafeId, Guid categoryId, [FromBody] UpsertMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var category = await _db.MenuCategories
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == categoryId && x.CafeId == cafeId, cancellationToken);
            if (category == null) return NotFound();

            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            var viDesc = request.Translations?.Vi?.Description;
            var enDesc = request.Translations?.En?.Description;
            if (string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
                return BadRequest(new { message = "Name (VI) và Name (EN) là bắt buộc." });

            category.Slug = request.Slug.Trim().ToLowerInvariant();
            category.Name = viName;
            category.Description = viDesc;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;

            UpsertCategoryTranslation(category, LanguageCode.Vi, viName, viDesc);
            UpsertCategoryTranslation(category, LanguageCode.En, enName, enDesc);

            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outTr =
                category.Translations.FirstOrDefault(t => t.Language == lang)
                ?? category.Translations.FirstOrDefault(t => t.Language == fallbackLang);

            return Ok(new MenuCategoryDto
            {
                Id = category.Id,
                CafeId = category.CafeId,
                Slug = category.Slug,
                Name = outTr?.Name ?? category.Name,
                Description = outTr?.Description ?? category.Description,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            });
        }

        [HttpDelete("{cafeId:guid}/categories/{categoryId:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid cafeId, Guid categoryId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var category = await _db.MenuCategories.FirstOrDefaultAsync(x => x.Id == categoryId && x.CafeId == cafeId, cancellationToken);
            if (category == null) return NotFound();

            _db.MenuCategories.Remove(category);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { message = "Deleted" });
        }

        [HttpGet("{cafeId:guid}/items")]
        public async Task<IActionResult> GetItems(Guid cafeId, [FromQuery] Guid? categoryId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;

            var query = _db.MenuItems.AsQueryable().Where(x => x.CafeId == cafeId);
            if (categoryId.HasValue) query = query.Where(x => x.CategoryId == categoryId.Value);

            var items = await query
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    CafeId = x.CafeId,
                    CategoryId = x.CategoryId,
                    Slug = x.Slug,
                    Name =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Name).FirstOrDefault()
                        ?? x.Name,
                    Description =
                        x.Translations.Where(t => t.Language == lang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Translations.Where(t => t.Language == fallbackLang).Select(t => t.Description).FirstOrDefault()
                        ?? x.Description,
                    ImageUrl = x.ImageUrl,
                    BasePrice = x.BasePrice,
                    Currency = x.Currency,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Ok(items);
        }

        [HttpPost("{cafeId:guid}/items")]
        public async Task<IActionResult> CreateItem(Guid cafeId, [FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var slug = request.Slug.Trim().ToLowerInvariant();
            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            var viDesc = request.Translations?.Vi?.Description;
            var enDesc = request.Translations?.En?.Description;
            if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
                return BadRequest(new { message = "Slug, Name (VI) và Name (EN) là bắt buộc." });

            var categoryOk = await _db.MenuCategories.AnyAsync(x => x.Id == request.CategoryId && x.CafeId == cafeId, cancellationToken);
            if (!categoryOk) return BadRequest(new { message = "CategoryId không hợp lệ." });

            var exists = await _db.MenuItems.AnyAsync(x => x.CafeId == cafeId && x.Slug == slug, cancellationToken);
            if (exists) return BadRequest(new { message = "Slug đã tồn tại trong quán." });

            var item = new MenuItem
            {
                CafeId = cafeId,
                CategoryId = request.CategoryId,
                Slug = slug,
                // legacy single-language columns (VI)
                Name = viName,
                Description = viDesc,
                BasePrice = request.BasePrice,
                Currency = request.Currency.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };

            item.Translations.Add(new MenuItemTranslation
            {
                MenuItemId = item.Id,
                Language = LanguageCode.Vi,
                Name = viName,
                Description = viDesc
            });
            item.Translations.Add(new MenuItemTranslation
            {
                MenuItemId = item.Id,
                Language = LanguageCode.En,
                Name = enName,
                Description = enDesc
            });

            _db.MenuItems.Add(item);
            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outTr =
                item.Translations.FirstOrDefault(t => t.Language == lang)
                ?? item.Translations.FirstOrDefault(t => t.Language == fallbackLang);

            return Ok(new MenuItemDto
            {
                Id = item.Id,
                CafeId = item.CafeId,
                CategoryId = item.CategoryId,
                Slug = item.Slug,
                Name = outTr?.Name ?? item.Name,
                Description = outTr?.Description ?? item.Description,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Currency = item.Currency,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            });
        }

        [HttpPut("{cafeId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> UpdateItem(Guid cafeId, Guid itemId, [FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var item = await _db.MenuItems
                .Include(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == itemId && x.CafeId == cafeId, cancellationToken);
            if (item == null) return NotFound();

            var categoryOk = await _db.MenuCategories.AnyAsync(x => x.Id == request.CategoryId && x.CafeId == cafeId, cancellationToken);
            if (!categoryOk) return BadRequest(new { message = "CategoryId không hợp lệ." });

            var viName = request.Translations?.Vi?.Name?.Trim() ?? string.Empty;
            var enName = request.Translations?.En?.Name?.Trim() ?? string.Empty;
            var viDesc = request.Translations?.Vi?.Description;
            var enDesc = request.Translations?.En?.Description;
            if (string.IsNullOrWhiteSpace(viName) || string.IsNullOrWhiteSpace(enName))
                return BadRequest(new { message = "Name (VI) và Name (EN) là bắt buộc." });

            item.CategoryId = request.CategoryId;
            item.Slug = request.Slug.Trim().ToLowerInvariant();
            item.Name = viName;
            item.Description = viDesc;
            item.BasePrice = request.BasePrice;
            item.Currency = request.Currency.Trim();
            item.DisplayOrder = request.DisplayOrder;
            item.IsActive = request.IsActive;

            UpsertItemTranslation(item, LanguageCode.Vi, viName, viDesc);
            UpsertItemTranslation(item, LanguageCode.En, enName, enDesc);

            await _db.SaveChangesAsync(cancellationToken);

            var lang = LanguageResolver.Resolve(Request);
            var fallbackLang = lang == LanguageCode.En ? LanguageCode.Vi : LanguageCode.En;
            var outTr =
                item.Translations.FirstOrDefault(t => t.Language == lang)
                ?? item.Translations.FirstOrDefault(t => t.Language == fallbackLang);

            return Ok(new MenuItemDto
            {
                Id = item.Id,
                CafeId = item.CafeId,
                CategoryId = item.CategoryId,
                Slug = item.Slug,
                Name = outTr?.Name ?? item.Name,
                Description = outTr?.Description ?? item.Description,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Currency = item.Currency,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            });
        }

        [HttpDelete("{cafeId:guid}/items/{itemId:guid}")]
        public async Task<IActionResult> DeleteItem(Guid cafeId, Guid itemId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var item = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == itemId && x.CafeId == cafeId, cancellationToken);
            if (item == null) return NotFound();

            _db.MenuItems.Remove(item);
            await _db.SaveChangesAsync(cancellationToken);
            return Ok(new { message = "Deleted" });
        }

        private static void UpsertCategoryTranslation(MenuCategory category, LanguageCode lang, string name, string? description)
        {
            var tr = category.Translations.FirstOrDefault(x => x.Language == lang);
            if (tr == null)
            {
                category.Translations.Add(new MenuCategoryTranslation
                {
                    MenuCategoryId = category.Id,
                    Language = lang,
                    Name = name,
                    Description = description
                });
                return;
            }

            tr.Name = name;
            tr.Description = description;
        }

        private static void UpsertItemTranslation(MenuItem item, LanguageCode lang, string name, string? description)
        {
            var tr = item.Translations.FirstOrDefault(x => x.Language == lang);
            if (tr == null)
            {
                item.Translations.Add(new MenuItemTranslation
                {
                    MenuItemId = item.Id,
                    Language = lang,
                    Name = name,
                    Description = description
                });
                return;
            }

            tr.Name = name;
            tr.Description = description;
        }
    }
}

