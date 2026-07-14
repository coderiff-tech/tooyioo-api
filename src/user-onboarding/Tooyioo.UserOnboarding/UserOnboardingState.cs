using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding;

public record UserOnboardingState
    : State<UserOnboardingState, UserOnboardingId>
{
    public string FirstName { get; private init; } = null!;
    public string LastName { get; private init; } = null!;
    public string Email { get; private init; } = null!;
    
    public UserOnboardingState()
    {
        On<UserOnboardingDomainEvents.V1.Initiated>(Initiated);
    }

    private static UserOnboardingState Initiated(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.Initiated domainEvent)
    {
        return state with
        {
            FirstName = domainEvent.Name,
            LastName = domainEvent.LastName,
            Email = domainEvent.Email
        };
    }
}