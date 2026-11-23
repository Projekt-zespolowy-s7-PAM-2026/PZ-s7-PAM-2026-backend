using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;
using Michi.API.Identity.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Michi.API.Identity.Seeding;

public class DevelopmentAdminSeeder(
    UserManager<ApplicationUser> userManager,
    IOptions<AdminSeedOptions> options,
    ILogger<DevelopmentAdminSeeder> logger)
{
    public async Task SeedAsync()
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            logger.LogWarning(
                "Pominięto seedowanie admina: brak ustawienia {Section}:Password.",
                AdminSeedOptions.SectionName);
            return;
        }

        var admin = await FindOrCreateAdminAsync(settings, settings.Password);
        await EnsureAdminRoleAsync(admin);
    }

    private async Task<ApplicationUser> FindOrCreateAdminAsync(AdminSeedOptions settings, string password)
    {
        var existing = await userManager.FindByEmailAsync(settings.Email);
        if (existing is not null)
        {
            return existing;
        }

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = settings.UserName,
            Email = settings.Email,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, password);
        result.ThrowIfFailed("Nie udało się utworzyć konta administratora");

        logger.LogInformation("Utworzono konto administratora deweloperskiego {Email}.", settings.Email);
        return admin;
    }

    private async Task EnsureAdminRoleAsync(ApplicationUser admin)
    {
        if (await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            return;
        }

        var result = await userManager.AddToRoleAsync(admin, AppRoles.Admin);
        result.ThrowIfFailed("Nie udało się przypisać roli Admin");
    }
}