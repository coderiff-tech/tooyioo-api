using Eventuous;
using Tooyioo.Profile.Contracts;

namespace Tooyioo.Profile.Domain.UniqueAlias;

public sealed record UniqueAliasState
    : State<UniqueAliasState, UniqueAliasId>
{
    public ProfileId? IdentityId { get; private init; }
    
    public UniqueAliasState()
    {
        On<UniqueExternalIdDomainEvents.V1.Claimed>(UniqueExternalIdentityClaimed);
    }

    private static UniqueAliasState UniqueExternalIdentityClaimed(
        UniqueAliasState state, 
        UniqueExternalIdDomainEvents.V1.Claimed domainEvent)
    {
        return state with
        {
            IdentityId = domainEvent.ProfileId
        };
    }
}