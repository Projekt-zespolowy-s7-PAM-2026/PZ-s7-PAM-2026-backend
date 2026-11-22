using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Michi.API.Identity.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(IdentityTables.Schema);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        RenameJoinTables(builder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private static void RenameJoinTables(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<Guid>>().ToTable(IdentityTables.UserRoles);
        builder.Entity<IdentityUserClaim<Guid>>().ToTable(IdentityTables.UserClaims);
        builder.Entity<IdentityUserLogin<Guid>>().ToTable(IdentityTables.UserLogins);
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable(IdentityTables.RoleClaims);
        builder.Entity<IdentityUserToken<Guid>>().ToTable(IdentityTables.UserTokens);
    }

    private void ApplyAuditTimestamps()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}