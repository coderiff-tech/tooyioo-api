using Eventuous;
using Tooiyoo.Identity.Contracts;

namespace Tooiyoo.Identity.Domain;

public sealed record IdentityState
    : State<IdentityState, IdentityId>
{
    public string FirstName { get; private init; } = null!;
    public string LastName { get; private init; } = null!;
    public string Email { get; private init; } = null!;
    public bool IsEmailVerified { get; private init; } = false;
    public string ExternalId { get; private init; } = null!;
    public string ExternalProvider { get; private init; } = null!;
    public string? Alias { get; private init; }
    public string? PhoneNumber { get; private init; }
    public bool IsProfileComplete { get; private init; } = false;
    
    public IdentityState()
    {
        On<IdentityDomainEvents.V1.Created>(IdentityCreated);
        On<IdentityDomainEvents.V1.EmailVerified>(IdentityEmailVerified);
        On<IdentityDomainEvents.V1.ExternalIdentityAssociated>(IdentityExternalIdAssociated);
        On<IdentityDomainEvents.V1.AliasSet>(IdentityAliasSet);
        On<IdentityDomainEvents.V1.PhoneNumberSet>(IdentityPhoneNumberSet);
        On<IdentityDomainEvents.V1.ProfileCompleted>(IdentityProfileCompleted);
    }

    private static IdentityState IdentityCreated(
        IdentityState state, 
        IdentityDomainEvents.V1.Created domainEvent)
    {
        return state with
        {
            FirstName = domainEvent.Name,
            LastName = domainEvent.LastName,
            Email = domainEvent.Email
        };
    }
    
    private static IdentityState IdentityEmailVerified(
        IdentityState state, 
        IdentityDomainEvents.V1.EmailVerified domainEvent)
    {
        return state with
        {
            IsEmailVerified = true
        };
    }
    
    private static IdentityState IdentityExternalIdAssociated(
        IdentityState state, 
        IdentityDomainEvents.V1.ExternalIdentityAssociated domainEvent)
    {
        return state with
        {
            ExternalId = domainEvent.ExternalId,
            ExternalProvider = domainEvent.ExternalProviderName
        };
    }
    
    private static IdentityState IdentityAliasSet(
        IdentityState state, 
        IdentityDomainEvents.V1.AliasSet domainEvent)
    {
        return state with
        {
            Alias = domainEvent.Alias
        };
    }
    
    private static IdentityState IdentityPhoneNumberSet(
        IdentityState state, 
        IdentityDomainEvents.V1.PhoneNumberSet domainEvent)
    {
        return state with
        {
            PhoneNumber = domainEvent.PhoneNumber
        };
    }
    
    private static IdentityState IdentityProfileCompleted(
        IdentityState state, 
        IdentityDomainEvents.V1.ProfileCompleted domainEvent)
    {
        return state with
        {
            IsProfileComplete = true
        };
    }
}