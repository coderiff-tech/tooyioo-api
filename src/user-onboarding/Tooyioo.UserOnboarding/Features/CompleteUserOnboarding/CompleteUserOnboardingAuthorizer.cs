using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.CompleteUserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding.Features.CompleteUserOnboarding;

public sealed class CompleteUserOnboardingAuthorizer
    : IRouteAuthorizer<CompleteUserOnboardingRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal, 
        CompleteUserOnboardingRoute route, 
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