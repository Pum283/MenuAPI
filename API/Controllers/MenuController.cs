using Application.Common.Interfaces;
using Application.DTOs.Menu;
using Domain.Entity;
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

        private static int GetUserId(ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : 0;
        }

        [HttpGet("{cafeId:int}/categories")]
        public async Task<IActionResult> GetCategories(int cafeId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var categories = await _db.MenuCategories
                .Where(x => x.CafeId == cafeId)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new MenuCategoryDto
                {
                    Id = x.Id,
                    CafeId = x.CafeId,
                    Slug = x.Slug,
                    Name = x.Name,
                    Description = x.Description,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Ok(categories);
        }

        [HttpPost("{cafeId:int}/categories")]
        public async Task<IActionResult> CreateCategory(int cafeId, [FromBody] UpsertMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var slug = request.Slug.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Slug và Name là bắt buộc." });

            var exists = await _db.MenuCategories.AnyAsync(x => x.CafeId == cafeId && x.Slug == slug, cancellationToken);
            if (exists) return BadRequest(new { message = "Slug đã tồn tại trong quán." });

            var category = new MenuCategory
            {
                CafeId = cafeId,
                Slug = slug,
                Name = request.Name.Trim(),
                Description = request.Description,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };

            _db.MenuCategories.Add(category);
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new MenuCategoryDto
            {
                Id = category.Id,
                CafeId = category.CafeId,
                Slug = category.Slug,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            });
        }

        [HttpPut("{cafeId:int}/categories/{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory(int cafeId, int categoryId, [FromBody] UpsertMenuCategoryRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var category = await _db.MenuCategories.FirstOrDefaultAsync(x => x.Id == categoryId && x.CafeId == cafeId, cancellationToken);
            if (category == null) return NotFound();

            category.Slug = request.Slug.Trim().ToLowerInvariant();
            category.Name = request.Name.Trim();
            category.Description = request.Description;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;

            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new MenuCategoryDto
            {
                Id = category.Id,
                CafeId = category.CafeId,
                Slug = category.Slug,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            });
        }

        [HttpDelete("{cafeId:int}/categories/{categoryId:int}")]
        public async Task<IActionResult> DeleteCategory(int cafeId, int categoryId, CancellationToken cancellationToken)
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

        [HttpGet("{cafeId:int}/items")]
        public async Task<IActionResult> GetItems(int cafeId, [FromQuery] int? categoryId, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

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
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    BasePrice = x.BasePrice,
                    Currency = x.Currency,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Ok(items);
        }

        [HttpPost("{cafeId:int}/items")]
        public async Task<IActionResult> CreateItem(int cafeId, [FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var slug = request.Slug.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Slug và Name là bắt buộc." });

            var categoryOk = await _db.MenuCategories.AnyAsync(x => x.Id == request.CategoryId && x.CafeId == cafeId, cancellationToken);
            if (!categoryOk) return BadRequest(new { message = "CategoryId không hợp lệ." });

            var exists = await _db.MenuItems.AnyAsync(x => x.CafeId == cafeId && x.Slug == slug, cancellationToken);
            if (exists) return BadRequest(new { message = "Slug đã tồn tại trong quán." });

            var item = new MenuItem
            {
                CafeId = cafeId,
                CategoryId = request.CategoryId,
                Slug = slug,
                Name = request.Name.Trim(),
                Description = request.Description,
                BasePrice = request.BasePrice,
                Currency = request.Currency.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };

            _db.MenuItems.Add(item);
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new MenuItemDto
            {
                Id = item.Id,
                CafeId = item.CafeId,
                CategoryId = item.CategoryId,
                Slug = item.Slug,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Currency = item.Currency,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            });
        }

        [HttpPut("{cafeId:int}/items/{itemId:int}")]
        public async Task<IActionResult> UpdateItem(int cafeId, int itemId, [FromBody] UpsertMenuItemRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken))
                return Forbid();

            var item = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == itemId && x.CafeId == cafeId, cancellationToken);
            if (item == null) return NotFound();

            var categoryOk = await _db.MenuCategories.AnyAsync(x => x.Id == request.CategoryId && x.CafeId == cafeId, cancellationToken);
            if (!categoryOk) return BadRequest(new { message = "CategoryId không hợp lệ." });

            item.CategoryId = request.CategoryId;
            item.Slug = request.Slug.Trim().ToLowerInvariant();
            item.Name = request.Name.Trim();
            item.Description = request.Description;
            item.BasePrice = request.BasePrice;
            item.Currency = request.Currency.Trim();
            item.DisplayOrder = request.DisplayOrder;
            item.IsActive = request.IsActive;

            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new MenuItemDto
            {
                Id = item.Id,
                CafeId = item.CafeId,
                CategoryId = item.CategoryId,
                Slug = item.Slug,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Currency = item.Currency,
                DisplayOrder = item.DisplayOrder,
                IsActive = item.IsActive
            });
        }

        [HttpDelete("{cafeId:int}/items/{itemId:int}")]
        public async Task<IActionResult> DeleteItem(int cafeId, int itemId, CancellationToken cancellationToken)
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
    }
}

