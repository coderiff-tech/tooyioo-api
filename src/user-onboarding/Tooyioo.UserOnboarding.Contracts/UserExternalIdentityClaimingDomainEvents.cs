using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class UserExternalIdentityClaimingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [EventType(Prefix + "UserExternalIdentityClaimed")]
        public record UserExternalIdentityClaimed(string UserOnboardingId);

        [EventType(Prefix + "UserExternalIdentityReleased")]
        public record UserExternalIdentityReleased(string UserOnboardingId);
    }
}
