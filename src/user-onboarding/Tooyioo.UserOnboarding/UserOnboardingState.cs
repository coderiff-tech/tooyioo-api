using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding;

public record UserOnboardingState
    : State<UserOnboardingState, UserOnboardingId>
{
    public string FirstName { get; private init; } = null!;
    public string LastName { get; private init; } = null!;
    public string Email { get; private init; } = null!;
    public bool IsEmailVerified { get; private init; }
    public string ExternalId { get; private init; } = null!;
    public string ExternalProvider { get; private init; } = null!;
    public string? Alias { get; private init; }
    public bool IsProfileComplete { get; private init; }
    
    public UserOnboardingState()
    {
        On<UserOnboardingDomainEvents.V1.UserOnboardingInitiated>(Initiated);
        On<UserOnboardingDomainEvents.V1.UserExternalIdAssociated>(UserExternalIdAssociated);
        On<UserOnboardingDomainEvents.V1.UserEmailVerified>(UserEmailVerified);
        On<UserOnboardingDomainEvents.V1.UserAliasChosen>(UserAliasChosen);
    }

    private static UserOnboardingState Initiated(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserOnboardingInitiated domainEvent)
    {
        return state with
        {
            FirstName = domainEvent.Name,
            LastName = domainEvent.LastName,
            Email = domainEvent.Email
        };
    }
    
    private static UserOnboardingState UserExternalIdAssociated(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserExternalIdAssociated domainEvent)
    {
        return state with
        {
            ExternalId = domainEvent.ExternalId,
            ExternalProvider = domainEvent.ExternalIdProvider
        };
    }
    
    private static UserOnboardingState UserEmailVerified(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserEmailVerified domainEvent)
    {
        return state with
        {
            IsEmailVerified = true
        };
    }
    
    private static UserOnboardingState UserAliasChosen(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserAliasChosen domainEvent)
    {
        return state with
        {
            Alias = domainEvent.Alias
        };
    }
}