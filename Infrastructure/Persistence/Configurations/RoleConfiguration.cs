using Domain.Entity;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasConversion<int>().IsRequired();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);

            builder.HasIndex(x => x.Code).IsUnique();

            // Seed base roles
            builder.HasData(
                new Role { Id = 1, Code = RoleCode.Admin, Name = "Admin" },
                new Role { Id = 2, Code = RoleCode.Brand, Name = "Brand" }
            );
        }
    }
}

