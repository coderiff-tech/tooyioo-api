using Eventuous;
using Tooyioo.Profile.Contracts;

namespace Tooyioo.Profile.Domain.UniqueAlias;

public sealed record UniqueAliasState
    : State<UniqueAliasState, UniqueAliasId>
{
    public ProfileId? IdentityId { get; private init; }
    
    public UniqueAliasState()
    {
        On<UniqueAliasDomainEvents.V1.Claimed>(UniqueAliasClaimed);
    }

    private static UniqueAliasState UniqueAliasClaimed(
        UniqueAliasState state, 
        UniqueAliasDomainEvents.V1.Claimed domainEvent)
    {
        return state with
        {
            IdentityId = domainEvent.ProfileId
        };
    }
}