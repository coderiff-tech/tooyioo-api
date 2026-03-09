using Eventuous;
using Tooiyoo.Identity.Contracts;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Domain;

public sealed class IdentityAggregate
    : Aggregate<IdentityState>
{
    public void Bootstrap(
        string name, 
        string lastName, 
        string email, 
        bool isEmailVerified, 
        string externalId,
        string externalProvider)
    {
        EnsureDoesntExist();
        
        var identityCreated = new IdentityDomainEvents.V1.Created(name, lastName, email);
        Apply(identityCreated);

        if (!string.IsNullOrWhiteSpace(externalId))
        {
            var identityExternalIdentityAssociated = 
                new IdentityDomainEvents.V1.ExternalIdentityAssociated(externalId, externalProvider);
            Apply(identityExternalIdentityAssociated);
        }

        if (!isEmailVerified)
        {
            return;
        }
        
        var identityEmailVerified = new IdentityDomainEvents.V1.EmailVerified();
        Apply(identityEmailVerified);
    }

    public void CompleteProfile(
        string alias, 
        string phoneNumber)
    {
        EnsureExists();

        if (State.IsProfileComplete)
        {
            return;
        }
        
        var identityAliasAssociated = new IdentityDomainEvents.V1.AliasSet(alias);
        Apply(identityAliasAssociated);

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            var identityPhoneNumberSet = new IdentityDomainEvents.V1.PhoneNumberSet(phoneNumber);
            Apply(identityPhoneNumberSet);
        }
        
        var identityProfileCompleted = new IdentityDomainEvents.V1.ProfileCompleted();
        Apply(identityProfileCompleted);
    }
}