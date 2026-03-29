namespace Tooyioo.Profile.Features.SetPhoneNumber.Contracts;

/// <summary>
/// Response returned after a successful profile phone number set
/// </summary>
/// <example>{"id": "00000000-0000-0000-0000-000000000001"}</example>
public sealed record SetProfilePhoneNumberResponse
{
    /// <summary>
    /// The unique identifier of the profile
    /// </summary>
    public required string Id { get; init; }
}