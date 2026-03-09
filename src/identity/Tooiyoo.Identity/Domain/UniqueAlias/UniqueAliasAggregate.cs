using Eventuous;
using Funzo;
using Tooiyoo.Identity.Contracts;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Domain.UniqueAlias;

public sealed class UniqueAliasAggregate
    : Aggregate<UniqueAliasState>
{
    public ClaimUniqueAliasResult Claim(IdentityId identityId)
    {
        if (State.IdentityId is not null)
        {
            return identityId == State.IdentityId 
                ? ClaimUniqueAliasResult.Ok() 
                : new UniqueAliasAlreadyClaimedError(State.IdentityId!);
        }
        
        var uniqueExternalIdentityClaimed = new UniqueAliasDomainEvents.V1.Claimed(identityId);
        Apply(uniqueExternalIdentityClaimed);
        return ClaimUniqueAliasResult.Ok();
    }
}

[Result<UniqueAliasAlreadyClaimedError>]
public partial class ClaimUniqueAliasResult;

public sealed record UniqueAliasAlreadyClaimedError(IdentityId IdentityAliasClaimedBy);