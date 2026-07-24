using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.User.Features.RetrieveUserSummaries.Contract;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.User.Features.RetrieveUserSummaries;

public sealed class RetrieveUserSummariesAuthorizer
    : IRouteAuthorizer<RetrieveUserSummariesRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        RetrieveUserSummariesRoute route,
        HttpContext http,
        CancellationToken ct)
    {
        var userIdOption = claimsPrincipal.GetClaim(Claims.UserId);
        if (!userIdOption.IsSome(out _))
        {
            return Task.FromResult(false);
        }

        // TODO only certain users should retrieve user summaries.
        return Task.FromResult(true);
    }
}
