using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid BrandRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql("NEWSEQUENTIALID()");

            builder.Property(x => x.Code).HasConversion<int>().IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.HasIndex(x => x.Code).IsUnique();

            // Seed base roles
            builder.HasData(
                new Role { Id = AdminRoleId, Code = RoleCode.Admin, Name = "Admin" },
                new Role { Id = BrandRoleId, Code = RoleCode.Brand, Name = "Brand" }
            );
        }
    }
}

