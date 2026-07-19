using System.Security.Claims;
using Tooyioo.Common;

namespace Tooyioo.Api.Infrastructure.Auth;

internal static class ExternalIdentityClaimsParser
{
    public static ExternalIdentity Parser(ClaimsPrincipal claimsPrincipal)
    {
        const string googleIssuer = "accounts.google.com";
        
        var issuerOption = claimsPrincipal.GetClaim(Claims.Issuer);
        if (!issuerOption.IsSome(out var iss))
        {
            throw new InvalidOperationException($"JWT is missing the '{Claims.Issuer}' claim");
        }

        var isGoogleIssuer =
            iss.Equals(googleIssuer, StringComparison.OrdinalIgnoreCase)
            || iss.Equals($"https://{googleIssuer}", StringComparison.OrdinalIgnoreCase);

        var externalIdentityProvider = isGoogleIssuer 
            ? ExternalIdentityProvider.Google 
            : throw new InvalidOperationException("Unsupported issuer");
        
        var subjectOption = claimsPrincipal.GetClaim(Claims.Sub);
        if (!subjectOption.IsSome(out var sub))
        {
            throw new InvalidOperationException($"JWT is missing the '{Claims.Sub}' claim");
        }

        var externalIdentity = new ExternalIdentity(sub, externalIdentityProvider, iss);
        return externalIdentity;
    }
}