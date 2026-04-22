using Application.DTOs.Cafe;
using Domain.Entity;
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
            var cafes = await _db.Cafes
                .OrderBy(x => x.Id)
                .Select(x => new CafeDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
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
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { message = "Code và Name là bắt buộc." });
            }

            var exists = await _db.Cafes.AnyAsync(x => x.Code == code, cancellationToken);
            if (exists) return BadRequest(new { message = "Code đã tồn tại." });

            var cafe = new Cafe
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = request.IsActive,
            };

            _db.Cafes.Add(cafe);
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new CafeDto
            {
                Id = cafe.Id,
                Code = cafe.Code,
                Name = cafe.Name,
                LogoUrl = cafe.LogoUrl,
                HeroImageUrl = cafe.HeroImageUrl,
                IsActive = cafe.IsActive
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpsertCafeRequest request, CancellationToken cancellationToken)
        {
            var cafe = await _db.Cafes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (cafe == null) return NotFound();

            cafe.Name = request.Name.Trim();
            cafe.IsActive = request.IsActive;
            await _db.SaveChangesAsync(cancellationToken);

            return Ok(new CafeDto
            {
                Id = cafe.Id,
                Code = cafe.Code,
                Name = cafe.Name,
                LogoUrl = cafe.LogoUrl,
                HeroImageUrl = cafe.HeroImageUrl,
                IsActive = cafe.IsActive
            });
        }
    }
}

