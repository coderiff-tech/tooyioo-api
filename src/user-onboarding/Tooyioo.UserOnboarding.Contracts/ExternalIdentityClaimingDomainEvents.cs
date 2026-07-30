using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class ExternalIdentityClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [EventType(Prefix + "ExternalIdentityClaimed")]
        public record ExternalIdentityClaimed(string UserOnboardingId);

        [EventType(Prefix + "ExternalIdentityReleased")]
        public record ExternalIdentityReleased(string UserOnboardingId);
    }
}
