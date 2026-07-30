using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class AliasClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [EventType(Prefix + "AliasClaimed")]
        public record AliasClaimed(string UserOnboardingId);

        [EventType(Prefix + "AliasReleased")]
        public record AliasReleased(string UserOnboardingId);
    }
}
