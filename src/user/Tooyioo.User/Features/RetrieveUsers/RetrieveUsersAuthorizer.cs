using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slicent.Application.Authorization;
using Tooyioo.Common;
using Tooyioo.User.Features.RetrieveUsers.Contract;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.User.Features.RetrieveUsers;

public sealed class RetrieveUsersAuthorizer
    : IRouteAuthorizer<RetrieveUsersRoute>
{
    public Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal, 
        RetrieveUsersRoute route, 
        HttpContext http, 
        CancellationToken ct)
    {
        var userIdOption = claimsPrincipal.GetClaim(Claims.UserId);
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!userIdOption.IsSome(out _))
        {
            return Task.FromResult(false);
        }
      
        // TODO only certain users could retrieve other users' data
        return Task.FromResult(true);
    }
}