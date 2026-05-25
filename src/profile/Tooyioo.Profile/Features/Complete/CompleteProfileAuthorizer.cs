using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.Profile.Features.Complete.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Complete;

public sealed class CompleteProfileAuthorizer
    : IRouteAuthorizer<CompleteProfileRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        CompleteProfileRoute route,
        HttpContext http,
        CancellationToken cancellationToken = default)
    {
        var subOption = claimsPrincipal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            return Task.FromResult(false);
        }
        var isSameProfile = route.ProfileId.ToString() == sub;
        return Task.FromResult(isSameProfile);
    }
}