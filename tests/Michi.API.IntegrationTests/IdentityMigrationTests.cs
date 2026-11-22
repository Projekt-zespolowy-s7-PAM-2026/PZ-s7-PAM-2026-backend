using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;
using Michi.API.Identity.Persistence;
using Michi.API.Identity.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Testcontainers.PostgreSql;

namespace Michi.API.IntegrationTests;

public class IdentityMigrationTests : IAsyncLifetime
{
    private const string AdminEmail = "admin@test.local";
    private const string AdminPassword = "Str0ng!Passw0rd#";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();

    private ServiceProvider _services = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                _postgres.GetConnectionString(),
                npgsql => npgsql.MigrationsHistoryTable(
                    IdentityTables.MigrationsHistory,
                    IdentityTables.Schema)));
        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        _services = services.BuildServiceProvider();

        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task Migration_SeedsUserAndAdminRoles()
    {
        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var roleNames = await dbContext.Roles.Select(role => role.Name).ToListAsync();

        Assert.Contains(AppRoles.User, roleNames);
        Assert.Contains(AppRoles.Admin, roleNames);
    }

    [Fact]
    public async Task Migration_StoresIdsAsUuidInRenamedTables()
    {
        await using var scope = _services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var idTypes = await dbContext.Database
            .SqlQueryRaw<string>(
                """
                SELECT data_type AS "Value" FROM information_schema.columns
                WHERE table_schema = 'identity'
                  AND table_name IN ('users', 'roles')
                  AND column_name = 'Id'
                """)
            .ToListAsync();

        var legacyTables = await dbContext.Database
            .SqlQueryRaw<string>(
                """
                SELECT table_name AS "Value" FROM information_schema.tables
                WHERE table_name LIKE 'AspNet%'
                """)
            .ToListAsync();

        Assert.Equal(2, idTypes.Count);
        Assert.All(idTypes, type => Assert.Equal("uuid", type));
        Assert.Empty(legacyTables);
    }

    [Fact]
    public async Task Seeder_InDevelopment_CreatesAdminWithAdminRoleAndIsIdempotent()
    {
        await using var scope = _services.CreateAsyncScope();
        var seeder = CreateSeeder(scope.ServiceProvider, Environments.Development, AdminEmail);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(AdminEmail);

        Assert.NotNull(admin);
        Assert.NotEqual(Guid.Empty, admin.Id);
        Assert.False(admin.IsBlocked);
        Assert.True(await userManager.IsInRoleAsync(admin, AppRoles.Admin));
    }

    [Fact]
    public async Task Seeder_OutsideDevelopment_DoesNotCreateAdmin()
    {
        const string productionEmail = "prod-admin@test.local";

        await using var scope = _services.CreateAsyncScope();
        var seeder = CreateSeeder(scope.ServiceProvider, Environments.Production, productionEmail);

        await seeder.SeedAsync();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        Assert.Null(await userManager.FindByEmailAsync(productionEmail));
    }

    private static IdentitySeeder CreateSeeder(
        IServiceProvider provider,
        string environmentName,
        string adminEmail)
    {
        var options = Options.Create(new AdminSeedOptions
        {
            Email = adminEmail,
            Password = AdminPassword,
        });

        var roleSeeder = new RoleSeeder(provider.GetRequiredService<RoleManager<ApplicationRole>>());
        var adminSeeder = new DevelopmentAdminSeeder(
            provider.GetRequiredService<UserManager<ApplicationUser>>(),
            options,
            NullLogger<DevelopmentAdminSeeder>.Instance);

        return new IdentitySeeder(roleSeeder, adminSeeder, new FakeHostEnvironment(environmentName));
    }

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;

        public string ApplicationName { get; set; } = "Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}