namespace Michi.API.Identity.Constants;

public static class IdentityTables
{
    public const string Schema = "identity";
    public const string MigrationsHistory = "__ef_migrations_history";

    public const string Users = "users";
    public const string Roles = "roles";
    public const string UserRoles = "user_roles";
    public const string UserClaims = "user_claims";
    public const string UserLogins = "user_logins";
    public const string RoleClaims = "role_claims";
    public const string UserTokens = "user_tokens";
}