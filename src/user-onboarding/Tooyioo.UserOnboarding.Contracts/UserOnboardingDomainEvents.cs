using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class UserOnboardingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";
        
        [EventType(Prefix + "UserOnboardingInitiated")]
        public record UserOnboardingInitiated(string Name, string LastName, string Email);
        
        [EventType(Prefix + "UserExternalIdentityAssociated")]
        public record UserExternalIdentityAssociated(string Id, string Provider, string Issuer);
        
        [EventType(Prefix + "UserEmailVerified")]
        public record UserEmailVerified;
        
        [EventType(Prefix + "UserAliasChosen")]
        public record UserAliasChosen(string Alias);
    }
}