using System.Security.Claims;
using Eventuous;
using Funzo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Slicent.EventStore;
using Tooiyoo.Identity.Domain.UniqueExternalIdentity;
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooiyoo.Api.Infrastructure.Auth;

internal class IdentityPrincipalCache
    : IIdentityPrincipalService
{
    private readonly IEventStore _eventStore;

    public IdentityPrincipalCache(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Option<ClaimsPrincipal>> GetByExternalId(string externalId)
    {
        ArgumentNullException.ThrowIfNull(externalId);
        
        // TODO Replace with proper read model and in memory cache and make aggregate and states again internal
        var tempAggregate = 
            await _eventStore.LoadAggregateOrNew<UniqueExternalIdentityAggregate, UniqueExternalIdentityState, UniqueExternalIdentityId>(
                externalId, CancellationToken.None);
        var identityId = tempAggregate.State.IdentityId;
        
        if (identityId is null)
        {
            return Option<ClaimsPrincipal>.None;
        }
        
        var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme, ClaimTypes.NameIdentifier, ClaimTypes.Role);
        
        identity.AddClaim(new Claim("sub", identityId.Value));
        
        return new ClaimsPrincipal(identity);
    }
}