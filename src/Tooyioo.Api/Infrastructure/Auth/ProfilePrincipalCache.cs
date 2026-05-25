using System.Security.Claims;
using Eventuous;
using Funzo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Slicent.EventStore;
using Tooyioo.Profile.Domain.UniqueExternalIdentity;

// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.Api.Infrastructure.Auth;

internal class ProfilePrincipalCache
    : IProfilePrincipalService
{
    private readonly IEventStore _eventStore;

    public ProfilePrincipalCache(IEventStore eventStore)
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
        var profileId = tempAggregate.State.ProfileId;
        
        if (profileId is null)
        {
            return Option<ClaimsPrincipal>.None;
        }
        
        var claimsIdentity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme, ClaimTypes.NameIdentifier, ClaimTypes.Role);
        
        claimsIdentity.AddClaim(new Claim("sub", profileId.Value));
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}