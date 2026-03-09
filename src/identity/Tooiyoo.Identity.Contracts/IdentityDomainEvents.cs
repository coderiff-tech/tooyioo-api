using Eventuous;

namespace Tooiyoo.Identity.Contracts;

public static class IdentityDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.Identity";
        
        [EventType(Prefix + "Created")]
        public record Created(string Name, string LastName, string Email);
        
        [EventType(Prefix + "ExternalIdentityAssociated")]
        public record ExternalIdentityAssociated(string ExternalId, string ExternalProviderName);
        
        [EventType(Prefix + "EmailVerified")]
        public record EmailVerified;
        
        [EventType(Prefix + "AliasSet")]
        public record AliasSet(string Alias);
        
        [EventType(Prefix + "PhoneNumberSet")]
        public record PhoneNumberSet(string PhoneNumber);
        
        [EventType(Prefix + "ProfileCompleted")]
        public record ProfileCompleted;
    }
}