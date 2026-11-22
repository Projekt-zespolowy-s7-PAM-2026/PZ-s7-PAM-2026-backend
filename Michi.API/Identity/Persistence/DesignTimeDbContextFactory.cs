using Michi.API.Identity.Constants;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Michi.API.Identity.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    private const string ConnectionStringVariable = "ConnectionStrings__Default";
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=michi_db;Username=michi;Password=design-time-only";

    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable)
                               ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable(
                    IdentityTables.MigrationsHistory,
                    IdentityTables.Schema))
            .Options;

        return new ApplicationDbContext(options);
    }
}