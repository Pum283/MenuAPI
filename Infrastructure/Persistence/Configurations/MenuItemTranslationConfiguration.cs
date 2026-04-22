using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class MenuItemTranslationConfiguration : IEntityTypeConfiguration<MenuItemTranslation>
    {
        public void Configure(EntityTypeBuilder<MenuItemTranslation> builder)
        {
            builder.ToTable("MenuItemTranslations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000);

            builder.HasIndex(x => new { x.MenuItemId, x.Language }).IsUnique();

            builder.HasOne(x => x.MenuItem)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

