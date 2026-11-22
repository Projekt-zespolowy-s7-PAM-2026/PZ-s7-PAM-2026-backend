namespace Michi.API.Identity.Seeding;

public class IdentitySeeder(
    RoleSeeder roleSeeder,
    DevelopmentAdminSeeder adminSeeder,
    IHostEnvironment environment)
{
    public async Task SeedAsync()
    {
        await roleSeeder.SeedAsync();

        if (environment.IsDevelopment())
        {
            await adminSeeder.SeedAsync();
        }
    }
}