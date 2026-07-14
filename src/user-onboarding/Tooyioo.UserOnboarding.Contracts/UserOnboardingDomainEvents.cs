using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class UserOnboardingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.UserOnboarding";
        
        [EventType(Prefix + "Initiated")]
        public record Initiated(string Name, string LastName, string Email);
        
        [EventType(Prefix + "ExternalIdAssociated")]
        public record ExternalIdAssociated(string ExternalId, string ExternalIdProvider);
        
        [EventType(Prefix + "EmailVerified")]
        public record EmailVerified;
    }
}