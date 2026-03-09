using System.Security.Claims;
using Funzo;

namespace Tooiyoo.Common;

public static class ClaimsPrincipalExtensions
{
    public static Option<string> GetSubClaim(this ClaimsPrincipal claimsPrincipal)
    {
        var sub = claimsPrincipal.FindFirstValue("sub");
        return sub is not null
            ? Option.Some(sub)
            : Option<string>.None;
    }
}