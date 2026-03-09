using System.Security.Claims;
using Funzo;

namespace Tooiyoo.Api.Infrastructure.Auth;

internal interface IIdentityPrincipalService
{
    public Task<Option<ClaimsPrincipal>> GetByExternalId(string externalId);
}