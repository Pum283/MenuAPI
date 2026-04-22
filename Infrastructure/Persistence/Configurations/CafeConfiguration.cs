using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CafeConfiguration : IEntityTypeConfiguration<Cafe>
    {
        public void Configure(EntityTypeBuilder<Cafe> builder)
        {
            builder.ToTable("Cafes");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

            builder.Property(x => x.LogoUrl).HasMaxLength(500);
            builder.Property(x => x.LogoPublicId).HasMaxLength(200);

            builder.Property(x => x.HeroImageUrl).HasMaxLength(500);
            builder.Property(x => x.HeroImagePublicId).HasMaxLength(200);

            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}

