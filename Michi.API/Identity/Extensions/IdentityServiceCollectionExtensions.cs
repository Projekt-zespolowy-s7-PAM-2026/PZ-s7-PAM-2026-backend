using Michi.API.Identity.Constants;
using Michi.API.Identity.Entities;
using Michi.API.Identity.Persistence;
using Michi.API.Identity.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Michi.API.Identity.Extensions;

public static class IdentityServiceCollectionExtensions
{
    private const string ConnectionStringName = "Default";

    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddIdentityPersistence(configuration)
            .AddIdentityCore()
            .AddIdentitySeeding(configuration);
    }

    private static IServiceCollection AddIdentityPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(ConnectionStringName),
                npgsql => npgsql.MigrationsHistoryTable(
                    IdentityTables.MigrationsHistory,
                    IdentityTables.Schema)));
    }

    private static IServiceCollection AddIdentityCore(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, ApplicationRole>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddIdentitySeeding(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AdminSeedOptions>(configuration.GetSection(AdminSeedOptions.SectionName));

        return services
            .AddScoped<RoleSeeder>()
            .AddScoped<DevelopmentAdminSeeder>()
            .AddScoped<IdentitySeeder>();
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 10;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    }
}