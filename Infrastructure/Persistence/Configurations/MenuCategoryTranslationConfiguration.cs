using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuCategoryTranslationConfiguration : IEntityTypeConfiguration<MenuCategoryTranslation>
    {
        public void Configure(EntityTypeBuilder<MenuCategoryTranslation> builder)
        {
            builder.ToTable("MenuCategoryTranslations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000);

            builder.HasIndex(x => new { x.MenuCategoryId, x.Language }).IsUnique();

            builder.HasOne(x => x.MenuCategory)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.MenuCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

