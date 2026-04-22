using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
    {
        public void Configure(EntityTypeBuilder<MenuCategory> builder)
        {
            builder.ToTable("MenuCategories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(800);

            builder.HasOne(x => x.Cafe)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.CafeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.CafeId, x.Slug }).IsUnique();
            builder.HasIndex(x => new { x.CafeId, x.DisplayOrder });
        }
    }
}

