namespace Tooiyoo.Identity.Features.CompleteProfile.Contracts;

/// <summary>
/// Response returned after a successful identity profile completed
/// </summary>
/// <example>{"id": "00000000-0000-0000-0000-000000000001"}</example>
public sealed record CompleteIdentityProfileResponse
{
    /// <summary>
    /// The unique identifier of the identity
    /// </summary>
    public required string Id { get; init; }
}