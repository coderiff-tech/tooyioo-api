using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Authorization;

public interface IPayloadAuthorizer<in TRequest>
{
    Task<bool> Authorize(
        ClaimsPrincipal user,
        TRequest request,
        HttpContext http,
        CancellationToken ct);
}