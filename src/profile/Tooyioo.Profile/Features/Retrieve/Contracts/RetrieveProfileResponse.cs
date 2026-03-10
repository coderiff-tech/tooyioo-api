namespace Tooyioo.Profile.Features.Retrieve.Contracts;

/// <summary>
/// Response returned after a successful retrieve profile
/// </summary>
/// <example>{"id": "00000000-0000-0000-0000-000000000001"}</example>
public sealed record RetrieveProfileResponse
{
    /// <summary>
    /// The unique identifier of the profile
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// Flag indicating whether the profile is complete or needs to be completed
    /// </summary>
    public required bool IsComplete { get; init; }
    
    /// <summary>
    /// The unique alias (if any)
    /// </summary>
    public required string Alias { get; init; }
    
    /// <summary>
    /// The email address
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// The phone number (if any)
    /// </summary>
    public required string PhoneNumber { get; init; }
}