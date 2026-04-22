using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CafeMemberConfiguration : IEntityTypeConfiguration<CafeMember>
    {
        public void Configure(EntityTypeBuilder<CafeMember> builder)
        {
            builder.ToTable("CafeMembers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).HasConversion<int>().HasDefaultValue(MemberStatus.Active);
            builder.Property(x => x.BlockedReason).HasMaxLength(500);

            builder.HasOne(x => x.Cafe)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.CafeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.CafeMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.CafeId, x.UserId }).IsUnique();
        }
    }
}

