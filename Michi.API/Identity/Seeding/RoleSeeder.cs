using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;
using Michi.API.Identity.Extensions;

using Microsoft.AspNetCore.Identity;

namespace Michi.API.Identity.Seeding;

public class RoleSeeder(RoleManager<ApplicationRole> roleManager)
{
    private static readonly string[] SystemRoles = [AppRoles.User, AppRoles.Admin];

    public async Task SeedAsync()
    {
        foreach (var roleName in SystemRoles)
        {
            await EnsureRoleExistsAsync(roleName);
        }
    }

    private async Task EnsureRoleExistsAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
        result.ThrowIfFailed($"Nie udało się utworzyć roli {roleName}");
    }
}