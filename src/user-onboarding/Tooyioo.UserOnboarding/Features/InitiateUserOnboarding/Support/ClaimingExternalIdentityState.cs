using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

internal sealed record ClaimingExternalIdentityState
    : State<ClaimingExternalIdentityState, ClaimingExternalIdentityId>
{
    public UserOnboardingId? UserOnboardingId { get; private init; }

    public ClaimingExternalIdentityState()
    {
        On<UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed>(Claimed);
    }

    private static ClaimingExternalIdentityState Claimed(
        ClaimingExternalIdentityState state,
        UserExternalIdentityClaimingDomainEvents.V1.UserExternalIdentityClaimed domainEvent)
    {
        return state with
        {
            UserOnboardingId = domainEvent.UserOnboardingId
        };
    }
}
  