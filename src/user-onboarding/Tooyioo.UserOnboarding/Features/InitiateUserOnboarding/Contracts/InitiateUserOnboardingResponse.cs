namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Contracts;

/// <summary>
/// Response returned after a successful initiate user onboarding request
/// </summary>
/// <remarks>
/// It contains the unique identifier of the user onboarding
/// </remarks>
/// <example>{"id": "00000000-0000-0000-0000-000000000001", "externalId": "12345", "externalIdProvider": "Google"}</example>
public sealed record InitiateUserOnboardingResponse
{
    /// <summary>
    /// The unique identifier of the user onboarding
    /// </summary>
    public required string UserOnboardingId { get; init; }
    
    /// <summary>
    /// The external identifier of the initiated user onboarding
    /// </summary>
    /// <remarks>
    /// This identifier is unique within the external provider
    /// </remarks>
    public required string ExternalId { get; init; }

    /// <summary>
    /// The external identity provider for the initiated user onboarding
    /// </summary>
    public required string ExternalIdProvider { get; init; }
}