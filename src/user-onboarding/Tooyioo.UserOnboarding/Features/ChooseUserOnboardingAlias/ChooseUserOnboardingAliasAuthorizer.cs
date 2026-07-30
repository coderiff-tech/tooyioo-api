using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias;

public sealed class ChooseUserOnboardingAliasAuthorizer
    : IRouteAuthorizer<ChooseUserOnboardingAliasRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        ChooseUserOnboardingAliasRoute route,
        HttpContext http,
        CancellationToken cancellationToken = default)
    {
        var userOnboardingIdOption = claimsPrincipal.GetClaim(Claims.UserOnboardingId);
        if (!userOnboardingIdOption.IsSome(out var userOnboardingId))
        {
            return Task.FromResult(false);
        }

        var isExpectedUserOnboardingId = route.UserOnboardingId.ToString() == userOnboardingId;
        return Task.FromResult(isExpectedUserOnboardingId);
    }
}
