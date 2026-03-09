using Eventuous;
using Funzo;
using Tooiyoo.Identity.Contracts;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Domain.UniqueExternalIdentity;

public sealed class UniqueExternalIdentityAggregate
    : Aggregate<UniqueExternalIdentityState>
{
    public ClaimUniqueExternalIdentityResult Claim(IdentityId identityId)
    {
        if (State.IdentityId is not null)
        {
            return identityId == State.IdentityId 
                ? ClaimUniqueExternalIdentityResult.Ok() 
                : new UniqueExternalIdentityAlreadyClaimedError(State.IdentityId!);
        }
        
        var uniqueExternalIdentityClaimed = new UniqueExternalIdentityDomainEvents.V1.Claimed(identityId);
        Apply(uniqueExternalIdentityClaimed);
        return ClaimUniqueExternalIdentityResult.Ok();
    }
}

[Result<UniqueExternalIdentityAlreadyClaimedError>]
public partial class ClaimUniqueExternalIdentityResult;

public sealed record UniqueExternalIdentityAlreadyClaimedError(IdentityId ExternalIdentityClaimedBy);