using Eventuous;

namespace Tooyioo.UserOnboarding;

public sealed record UserOnboardingId(string Value)
    : Id(Value)
{
    public static UserOnboardingId New() => new(Guid.NewGuid().ToString());
    public static implicit operator UserOnboardingId(string id) => new(id);
    public static implicit operator string(UserOnboardingId id) => id.Value;
}