using Eventuous;
using Tooiyoo.Identity.Contracts;

namespace Tooiyoo.Identity.Domain.UniqueAlias;

public sealed record UniqueAliasState
    : State<UniqueAliasState, UniqueAliasId>
{
    public IdentityId? IdentityId { get; private init; }
    
    public UniqueAliasState()
    {
        On<UniqueExternalIdentityDomainEvents.V1.Claimed>(UniqueExternalIdentityClaimed);
    }

    private static UniqueAliasState UniqueExternalIdentityClaimed(
        UniqueAliasState state, 
        UniqueExternalIdentityDomainEvents.V1.Claimed domainEvent)
    {
        return state with
        {
            IdentityId = domainEvent.IdentityId
        };
    }
}