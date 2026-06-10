using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Authorization;

public interface IRouteAuthorizer<in TRoute>
{
    Task<bool> Authorize(
        ClaimsPrincipal claimsPrincipal,
        TRoute route,
        HttpContext http,
        CancellationToken ct);
}