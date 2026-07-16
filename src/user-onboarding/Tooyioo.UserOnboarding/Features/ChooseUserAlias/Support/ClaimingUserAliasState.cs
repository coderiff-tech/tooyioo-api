using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias.Support;

internal sealed record ClaimingUserAliasState
    : State<ClaimingUserAliasState, ClaimingUserAliasId>
{
    public UserOnboardingId? UserOnboardingId { get; private init; }

    public ClaimingUserAliasState()
    {
        On<UserAliasClaimingDomainEvents.V1.UserAliasClaimed>(Claimed);
    }

    private static ClaimingUserAliasState Claimed(
        ClaimingUserAliasState state,
        UserAliasClaimingDomainEvents.V1.UserAliasClaimed domainEvent)
    {
        return state with
        {
            UserOnboardingId = domainEvent.UserOnboardingId
        };
    }
}
  