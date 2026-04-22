using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public static readonly Guid AdminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(300);

            builder.Property(x => x.Status).HasConversion<int>().HasDefaultValue(UserStatus.Active);
            builder.Property(x => x.BlockedReason).HasMaxLength(500);

            // Seed admin user
            builder.HasData(new User
            {
                Id = AdminUserId,
                Email = "admin@gmail.com",
                PasswordHash = "$2a$11$iV9hWKbffZV/dVcS/MVViu.l/LBM7Jc95XBVSESd0pD91Yc5bSH.a",
                Status = UserStatus.Active,
                BlockedReason = null,
                BlockedAt = null,
                IsDeleted = false,
                DeletedBy = null,
                DeletedDate = null,
                CreatedDate = new DateTime(2026, 4, 22, 0, 0, 0, DateTimeKind.Utc),
                UpdatedDate = null,
                CreatedBy = "seed",
                UpdatedBy = null
            });
        }
    }
}

