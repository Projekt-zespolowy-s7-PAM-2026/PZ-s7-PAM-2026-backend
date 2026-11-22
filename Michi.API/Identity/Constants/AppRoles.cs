namespace Michi.API.Identity.Constants;

public static class AppRoles
{
    public const string User = "User";
    public const string Admin = "Admin";

    public static readonly Guid UserId = new("6f1c2a4e-3b5d-4c7a-9d10-0a1b2c3d4e01");
    public static readonly Guid AdminId = new("6f1c2a4e-3b5d-4c7a-9d10-0a1b2c3d4e02");

    public const string UserConcurrencyStamp = "b5a0f2c1-0000-4000-8000-000000000001";
    public const string AdminConcurrencyStamp = "b5a0f2c1-0000-4000-8000-000000000002";
}