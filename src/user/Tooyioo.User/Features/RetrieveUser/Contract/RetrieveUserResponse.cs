// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Tooyioo.User.Features.RetrieveUser.Contract;

/// <summary>
/// Response returned after a successful retrieve profile
/// </summary>
/// <example>
/// {"id": "00000000-0000-0000-0000-000000000002", "alias": "joe-bloggs-spain_123", "name": "Joe", "lastName": "Bloggs",
/// "email": "joe@test.com",  "isEmailVerified": true, "phoneNumber": "+34644000000", "externalId": "12345",
/// "externalProvider": "Google", "userOnboardingId": "00000000-0000-0000-0000-000000000001",
/// "userOnboardingInitiatedAt": "2026-01-01T00:00:00.123Z", "createdAt": "2026-01-01T00:00:00.123Z",
/// "lastModifiedAt": "2026-01-01T00:00:00.123Z", "termsAndConditionsVersion": "v1"}"
/// </example>
public sealed record RetrieveUserResponse
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The alias
    /// </summary>
    public required string Alias { get; init; }
    
    /// <summary>
    /// The name
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// The last name
    /// </summary>
    public required string LastName { get; init; }
    
    /// <summary>
    /// The email address
    /// </summary>
    public required string Email { get; init; }
    
    /// <summary>
    /// The email verification status
    /// </summary>
    public required bool IsEmailVerified { get; init; }
    
    /// <summary>
    /// The phone number (if any)
    /// </summary>
    public required string PhoneNumber { get; init; }
    
    /// <summary>
    /// The external Id
    /// </summary>
    public required string ExternalId { get; init; }
    
    /// <summary>
    /// The external provider
    /// </summary>
    public required string ExternalProvider { get; init; }
    
    /// <summary>
    /// The user onboarding identifier that originated the user
    /// </summary>
    public required string UserOnboardingId { get; init; }
    
    /// <summary>
    /// The user onboarding initiation date
    /// </summary>
    public required string UserOnboardingInitiatedAt { get; init; }
    
    /// <summary>
    /// Creation date
    /// </summary>
    public required DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Last modification date
    /// </summary>
    public required DateTime LastModifiedAt { get; init; }
    
    /// <summary>
    /// Terms and Conditions version the user acknowledged
    /// </summary>
    public required string TermsAndConditionsVersion { get; init; }
}