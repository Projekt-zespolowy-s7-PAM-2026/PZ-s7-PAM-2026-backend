using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Michi.API.Identity.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable(IdentityTables.Users);

        builder.Property(user => user.CreatedAt).IsRequired();
        builder.Property(user => user.IsBlocked).HasDefaultValue(false);
    }
}