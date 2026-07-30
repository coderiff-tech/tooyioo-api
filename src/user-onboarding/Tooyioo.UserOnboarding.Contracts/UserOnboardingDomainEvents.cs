using Eventuous;

namespace Tooyioo.UserOnboarding.Contracts;

public static class UserOnboardingDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.";

        [EventType(Prefix + "UserOnboardingInitiated")]
        public record UserOnboardingInitiated(string Name, string LastName, string Email);

        [EventType(Prefix + "UserOnboardingExternalIdentityAssociated")]
        public record UserOnboardingExternalIdentityAssociated(string Id, string Provider, string Issuer);

        [EventType(Prefix + "UserOnboardingEmailVerified")]
        public record UserOnboardingEmailVerified;

        [EventType(Prefix + "UserOnboardingAliasChosen")]
        public record UserOnboardingAliasChosen(string Alias);

        [EventType(Prefix + "UserOnboardingCompleted")]
        public record UserOnboardingCompleted(string UserId, string TermsAndConditionsVersion);

        [EventType(Prefix + "UserOnboardingCanceled")]
        public record UserOnboardingCanceled(string Reason);
    }
}
