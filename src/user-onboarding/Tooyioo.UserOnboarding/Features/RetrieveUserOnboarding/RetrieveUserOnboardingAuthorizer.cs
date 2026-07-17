using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding;

public sealed class RetrieveUserOnboardingAuthorizer
    : IRouteAuthorizer<RetrieveUserOnboardingRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal, 
        RetrieveUserOnboardingRoute route, 
        HttpContext http, 
        CancellationToken ct)
    {
        var subOption = claimsPrincipal.GetSubClaim();
        if (!subOption.IsSome(out var sub))
        {
            return Task.FromResult(false);
        }
        var isExpectedUserOnboardingId = route.UserOnboardingId.ToString() == sub;
        return Task.FromResult(isExpectedUserOnboardingId);
    }
}