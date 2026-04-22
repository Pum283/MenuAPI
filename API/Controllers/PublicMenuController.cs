using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var cafe = await _db.Cafes.FirstOrDefaultAsync(x => x.Code == code && x.IsActive, cancellationToken);
            if (cafe == null) return NotFound();

            var categories = await _db.MenuCategories
                .Where(x => x.CafeId == cafe.Id && x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new
                {
                    x.Id,
                    x.Slug,
                    x.Name,
                    x.Description,
                    x.DisplayOrder,
                    items = x.Items
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new
                        {
                            i.Id,
                            i.Slug,
                            i.Name,
                            i.Description,
                            i.ImageUrl,
                            i.BasePrice,
                            i.Currency,
                            i.DisplayOrder
                        })
                })
                .ToListAsync(cancellationToken);

            return Ok(new
            {
                cafe = new
                {
                    cafe.Id,
                    cafe.Code,
                    cafe.Name,
                    cafe.LogoUrl,
                    cafe.HeroImageUrl
                },
                categories
            });
        }
    }
}

