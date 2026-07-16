namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;

/// <summary>
/// Response returned after successfully choosing user alias
/// </summary>
/// <example>{"id": "00000000-0000-0000-0000-000000000001"}</example>
public sealed record ChooseUserAliasResponse
{
    /// <summary>
    /// The unique identifier of the user onboarding
    /// </summary>
    public required string UserOnboardingId { get; init; }
}