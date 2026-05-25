using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.Profile.Features.SetPhoneNumber.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.SetPhoneNumber;

public sealed class SetProfilePhoneNumberAuthorizer
    : IRouteAuthorizer<SetProfilePhoneNumberRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        SetProfilePhoneNumberRoute route,
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