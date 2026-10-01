using Slicent.EventStore;

namespace Tooyioo.UserOnboarding.Contracts;

public static class ExternalIdentityClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [DomainEventType(Prefix + "ExternalIdentityClaimed")]
        public record ExternalIdentityClaimed(string UserOnboardingId);

        [DomainEventType(Prefix + "ExternalIdentityReleased")]
        public record ExternalIdentityReleased(string UserOnboardingId);
    }
}
