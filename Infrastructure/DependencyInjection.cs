using Application.Common.Interfaces;
using Application.Interfaces;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("MenuAPIDb"));
            }

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Common Services
            services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();

            // Auth / access
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICafeAccessService, CafeAccessService>();

            // Media (Cloudinary)
            services.AddScoped<IMediaService, CloudinaryMediaService>();

            return services;
        }
    }
}

