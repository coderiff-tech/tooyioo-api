using System.Security.Claims;
using Funzo;

namespace Tooyioo.Api.Infrastructure.Auth;

internal interface IProfilePrincipalService
{
    public Task<Option<ClaimsPrincipal>> GetByExternalId(string externalId);
}