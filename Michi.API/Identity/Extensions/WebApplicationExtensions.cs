using Michi.API.Identity.Persistence;
using Michi.API.Identity.Seeding;

using Microsoft.EntityFrameworkCore;

namespace Michi.API.Identity.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeIdentityAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        await using var scope = app.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        await seeder.SeedAsync();
    }
}