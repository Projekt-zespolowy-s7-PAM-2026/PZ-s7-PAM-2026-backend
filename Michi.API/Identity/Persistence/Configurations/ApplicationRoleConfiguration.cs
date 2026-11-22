using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Michi.API.Identity.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable(IdentityTables.Roles);

        builder.HasData(
            CreateRole(AppRoles.UserId, AppRoles.User, AppRoles.UserConcurrencyStamp),
            CreateRole(AppRoles.AdminId, AppRoles.Admin, AppRoles.AdminConcurrencyStamp));
    }

    private static ApplicationRole CreateRole(Guid id, string name, string concurrencyStamp) => new()
    {
        Id = id,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = concurrencyStamp,
    };
}