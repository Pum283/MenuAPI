using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class MediaController : ApiController
    {
        private readonly AppDbContext _db;
        private readonly ICafeAccessService _access;
        private readonly IMediaService _media;

        public MediaController(AppDbContext db, ICafeAccessService access, IMediaService media)
        {
            _db = db;
            _access = access;
            _media = media;
        }

        private static Guid GetUserId(ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }

        [HttpPut("cafes/{cafeId:guid}/logo")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> SetCafeLogo(Guid cafeId, IFormFile file, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken)) return Forbid();

            var cafe = await _db.Cafes.FirstOrDefaultAsync(x => x.Id == cafeId, cancellationToken);
            if (cafe == null) return NotFound();

            await using var stream = file.OpenReadStream();
            var (url, publicId) = await _media.UploadImageAsync(stream, file.FileName, cafe.LogoPublicId, cancellationToken);

            cafe.LogoUrl = url;
            cafe.LogoPublicId = publicId;
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new { cafe.Id, cafe.LogoUrl, cafe.LogoPublicId });
        }

        [HttpPut("cafes/{cafeId:guid}/hero")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> SetCafeHero(Guid cafeId, IFormFile file, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);
            if (!await _access.CanManageCafeAsync(userId, cafeId, cancellationToken)) return Forbid();

            var cafe = await _db.Cafes.FirstOrDefaultAsync(x => x.Id == cafeId, cancellationToken);
            if (cafe == null) return NotFound();

            await using var stream = file.OpenReadStream();
            var (url, publicId) = await _media.UploadImageAsync(stream, file.FileName, cafe.HeroImagePublicId, cancellationToken);

            cafe.HeroImageUrl = url;
            cafe.HeroImagePublicId = publicId;
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new { cafe.Id, cafe.HeroImageUrl, cafe.HeroImagePublicId });
        }

        [HttpPut("menu-items/{itemId:guid}/image")]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> SetMenuItemImage(Guid itemId, IFormFile file, CancellationToken cancellationToken)
        {
            var userId = GetUserId(User);

            var item = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == itemId, cancellationToken);
            if (item == null) return NotFound();

            if (!await _access.CanManageCafeAsync(userId, item.CafeId, cancellationToken)) return Forbid();

            await using var stream = file.OpenReadStream();
            var (url, publicId) = await _media.UploadImageAsync(stream, file.FileName, item.ImagePublicId, cancellationToken);

            item.ImageUrl = url;
            item.ImagePublicId = publicId;
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new { item.Id, item.ImageUrl, item.ImagePublicId });
        }
    }
}

