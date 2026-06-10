using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Authorization;

public interface IRoutePayloadAuthorizer<in TRoute, in TRequest>
{
    Task<bool> Authorize(
        ClaimsPrincipal user,
        TRoute route,
        TRequest request,
        HttpContext http,
        CancellationToken ct);
}