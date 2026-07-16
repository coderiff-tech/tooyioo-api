using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias;

public sealed class ChooseUserAliasAuthorizer
    : IRouteAuthorizer<ChooseUserAliasRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        ChooseUserAliasRoute route,
        HttpContext http,
        CancellationToken cancellationToken = default)
    {
        var subOption = claimsPrincipal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            return Task.FromResult(false);
        }
        var isSameProfile = route.UserOnboardingId.ToString() == sub;
        return Task.FromResult(isSameProfile);
    }
}