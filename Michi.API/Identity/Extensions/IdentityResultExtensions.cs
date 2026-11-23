using Microsoft.AspNetCore.Identity;

namespace Michi.API.Identity.Extensions;

public static class IdentityResultExtensions
{
    public static void ThrowIfFailed(this IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(", ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message}: {errors}");
    }
}