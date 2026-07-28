using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.CancelUserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.CancelUserOnboarding;

public sealed class CancelUserOnboardingAuthorizer
    : IRouteAuthorizer<CancelUserOnboardingRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        CancelUserOnboardingRoute route,
        HttpContext http,
        CancellationToken ct)
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
