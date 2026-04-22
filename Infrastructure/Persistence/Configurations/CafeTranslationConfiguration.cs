using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CafeTranslationConfiguration : IEntityTypeConfiguration<CafeTranslation>
    {
        public void Configure(EntityTypeBuilder<CafeTranslation> builder)
        {
            builder.ToTable("CafeTranslations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();

            builder.HasIndex(x => new { x.CafeId, x.Language }).IsUnique();

            builder.HasOne(x => x.Cafe)
                .WithMany(x => x.Translations)
                .HasForeignKey(x => x.CafeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

