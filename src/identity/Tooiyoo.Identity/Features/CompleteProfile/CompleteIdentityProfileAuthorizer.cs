using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooiyoo.Common;
using Tooiyoo.Identity.Features.CompleteProfile.Contracts;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Features.CompleteProfile;

public sealed class CompleteIdentityProfileAuthorizer
    : IRouteAuthorizer<CompleteIdentityProfileRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        CompleteIdentityProfileRoute route,
        HttpContext http,
        CancellationToken cancellationToken = default)
    {
        var subOption = claimsPrincipal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            return Task.FromResult(false);
        }
        var isSameIdentity = route.IdentityId.ToString() == sub;
        return Task.FromResult(isSameIdentity);
    }
}