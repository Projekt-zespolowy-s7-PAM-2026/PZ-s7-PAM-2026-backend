using Microsoft.AspNetCore.Identity;

namespace Michi.API.Identity.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsBlocked { get; set; }
}