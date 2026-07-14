using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class ClaimingAliasDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1. ClaimingAlias";
        
        [EventType(Prefix + "Claimed")]
        public record Claimed(string OnboardingId);
    }
}