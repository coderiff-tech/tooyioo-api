using Eventuous;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding;

public record UserOnboardingState
    : State<UserOnboardingState, UserOnboardingId>
{
    public string Name { get; private init; } = null!;
    public string LastName { get; private init; } = null!;
    public string Email { get; private init; } = null!;
    public bool IsEmailVerified { get; private init; }
    public string ExternalId { get; private init; } = null!;
    public string ExternalProvider { get; private init; } = null!;
    public string ExternalIssuer { get; private init; } = null!;
    public string? Alias { get; private init; }
    public bool IsComplete { get; private init; }
    public string? UserId { get; private init; }
    public bool IsCanceled { get; private init; }
    public string? CancellationReason { get; private init; }
    
    public UserOnboardingState()
    {
        On<UserOnboardingDomainEvents.V1.UserOnboardingInitiated>(Initiated);
        On<UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated>(UserExternalIdAssociated);
        On<UserOnboardingDomainEvents.V1.UserEmailVerified>(UserEmailVerified);
        On<UserOnboardingDomainEvents.V1.UserAliasChosen>(UserAliasChosen);
        On<UserOnboardingDomainEvents.V1.UserOnboardingCompleted>(UserOnboardingCompleted);
        On<UserOnboardingDomainEvents.V1.UserOnboardingCanceled>(UserOnboardingCanceled);
    }

    private static UserOnboardingState Initiated(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserOnboardingInitiated domainEvent)
    {
        return state with
        {
            Name = domainEvent.Name,
            LastName = domainEvent.LastName,
            Email = domainEvent.Email
        };
    }
    
    private static UserOnboardingState UserExternalIdAssociated(
        UserOnboardingState state, 
        UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated domainEvent)
    {
        return state with
        {
            ExternalId = domainEvent.Id,
            ExternalProvider = domainEvent.Provider,
            ExternalIssuer = domainEvent.Issuer
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

    private static UserOnboardingState UserOnboardingCompleted(
        UserOnboardingState state,
        UserOnboardingDomainEvents.V1.UserOnboardingCompleted domainEvent)
    {
        return state with
        {
            IsComplete = true,
            UserId = domainEvent.UserId
        };
    }

    private static UserOnboardingState UserOnboardingCanceled(
        UserOnboardingState state,
        UserOnboardingDomainEvents.V1.UserOnboardingCanceled domainEvent)
    {
        return state with
        {
            IsCanceled = true,
            CancellationReason = domainEvent.Reason
        };
    }
}
