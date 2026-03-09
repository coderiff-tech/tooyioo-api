using Eventuous;
using Tooiyoo.Identity.Contracts;

namespace Tooiyoo.Identity.Domain.UniqueExternalIdentity;

public sealed record UniqueExternalIdentityState
    : State<UniqueExternalIdentityState, UniqueExternalIdentityId>
{
    public IdentityId? IdentityId { get; private init; }
    
    public UniqueExternalIdentityState()
    {
        On<UniqueExternalIdentityDomainEvents.V1.Claimed>(UniqueExternalIdentityClaimed);
    }

    private static UniqueExternalIdentityState UniqueExternalIdentityClaimed(
        UniqueExternalIdentityState state, 
        UniqueExternalIdentityDomainEvents.V1.Claimed domainEvent)
    {
        return state with
        {
            IdentityId = domainEvent.IdentityId
        };
    }
}