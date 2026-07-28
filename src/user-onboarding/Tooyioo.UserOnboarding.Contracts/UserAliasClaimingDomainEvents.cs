using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class UserAliasClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [EventType(Prefix + "UserAliasClaimed")]
        public record UserAliasClaimed(string UserOnboardingId);

        [EventType(Prefix + "UserAliasReleased")]
        public record UserAliasReleased(string UserOnboardingId);
    }
}
