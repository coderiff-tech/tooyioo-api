using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class ClaimingExternalIdentityDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1. ClaimingExternalIdentity";
        
        [EventType(Prefix + "Claimed")]
        public record Claimed(string OnboardingId);
    }
}