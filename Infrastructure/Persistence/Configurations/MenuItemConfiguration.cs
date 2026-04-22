using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.ToTable("MenuItems");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(1200);

            builder.Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Property(x => x.ImagePublicId).HasMaxLength(200);

            builder.Property(x => x.BasePrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("VND");

            builder.HasOne(x => x.Cafe)
                .WithMany()
                .HasForeignKey(x => x.CafeId)
                // Avoid multiple cascade paths: Cafes -> MenuCategories -> MenuItems already cascades.
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.CafeId, x.Slug }).IsUnique();
            builder.HasIndex(x => new { x.CafeId, x.CategoryId, x.DisplayOrder });
        }
    }
}

