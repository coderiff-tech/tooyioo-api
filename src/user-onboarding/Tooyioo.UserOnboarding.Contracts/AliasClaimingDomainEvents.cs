using Slicent.EventStore;

namespace Tooyioo.UserOnboarding.Contracts;

public static class AliasClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [DomainEventType(Prefix + "AliasClaimed")]
        public record AliasClaimed(string UserOnboardingId);

        [DomainEventType(Prefix + "AliasReleased")]
        public record AliasReleased(string UserOnboardingId);
    }
}
