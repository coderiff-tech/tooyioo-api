using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding;

public sealed record ClaimingExternalIdentityState
    : State<ClaimingExternalIdentityState, ClaimingExternalIdentityId>
{
    public UserOnboardingId? OnboardingId { get; private init; }

    public ClaimingExternalIdentityState()
    {
        On<ClaimingExternalIdentityDomainEvents.V1.Claimed>(Claimed);
    }

    private static ClaimingExternalIdentityState Claimed(
        ClaimingExternalIdentityState state,
        ClaimingExternalIdentityDomainEvents.V1.Claimed domainEvent)
    {
        return state with
        {
            OnboardingId = domainEvent.OnboardingId
        };
    }
}
  