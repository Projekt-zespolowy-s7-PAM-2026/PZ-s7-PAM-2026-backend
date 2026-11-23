namespace Michi.API.Identity.Seeding;

public class AdminSeedOptions
{
    public const string SectionName = "Seed:Admin";

    public string Email { get; set; } = "admin@localhost";

    public string UserName { get; set; } = "admin";

    public string? Password { get; set; }
}