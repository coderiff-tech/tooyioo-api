using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias.Support;

internal sealed record ClaimingAliasState
    : State<ClaimingAliasState, ClaimingAliasId>
{
    public UserOnboardingId? UserOnboardingId { get; private init; }

    public ClaimingAliasState()
    {
        On<AliasClaimingDomainEvents.V1.AliasClaimed>(Claimed);
        On<AliasClaimingDomainEvents.V1.AliasReleased>(Released);
    }

    private static ClaimingAliasState Claimed(
        ClaimingAliasState state,
        AliasClaimingDomainEvents.V1.AliasClaimed domainEvent)
    {
        return state with
        {
            UserOnboardingId = domainEvent.UserOnboardingId
        };
    }

    private static ClaimingAliasState Released(
        ClaimingAliasState state,
        AliasClaimingDomainEvents.V1.AliasReleased domainEvent)
    {
        return state with
        {
            UserOnboardingId = null
        };
    }
}
