using AuthServer.Domain.Entities;
using AuthServerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authserver.Infrıastructure.Persistence.Configuration
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable("AppRoles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            var adminId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
            var userId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f23456789012");
            var staffId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-345678901234");
            var managerId = Guid.Parse("d4e5f6a7-b8c9-0123-def0-456789012345");

            builder.HasData(
                new AppRole
                {
                    Id = adminId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new AppRole
                {
                    Id = userId,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new AppRole
                {
                    Id = staffId,
                    Name = "Staff",
                    NormalizedName = "STAFF"
                },
                new AppRole
                {
                    Id = managerId,
                    Name = "Manager",
                    NormalizedName = "MANAGER"
                }
            );
        }
    }
}
