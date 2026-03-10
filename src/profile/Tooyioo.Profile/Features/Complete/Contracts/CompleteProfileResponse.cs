namespace Tooyioo.Profile.Features.Complete.Contracts;

/// <summary>
/// Response returned after a successful profile completed
/// </summary>
/// <example>{"id": "00000000-0000-0000-0000-000000000001"}</example>
public sealed record CompleteProfileResponse
{
    /// <summary>
    /// The unique identifier of the profile
    /// </summary>
    public required string Id { get; init; }
}