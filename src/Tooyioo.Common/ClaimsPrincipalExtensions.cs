using System.Security.Claims;
using Funzo;
// ReSharper disable ConvertToExtensionBlock

namespace Tooyioo.Common;

public static class ClaimsPrincipalExtensions
{
    public static Option<string> GetClaim(this ClaimsPrincipal claimsPrincipal, string claim)
    {
        var sub = claimsPrincipal.FindFirstValue(claim);
        return sub is not null
            ? Option.Some(sub)
            : Option<string>.None;
    }
}