// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

using Eventuous;

namespace Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias.Support;

internal sealed record ClaimingAliasId(string Value)
    : Id(Value)
{
    public static implicit operator ClaimingAliasId(string id) => new(id);
    public static implicit operator string(ClaimingAliasId id) => id.Value;
}