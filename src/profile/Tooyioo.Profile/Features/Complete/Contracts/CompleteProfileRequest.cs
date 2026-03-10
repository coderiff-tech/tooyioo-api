using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;
using Tooyioo.Profile.Features.Complete.Support;

namespace Tooyioo.Profile.Features.Complete.Contracts;

/// <summary>
/// Request payload used to complete the profile
/// </summary>
/// <example>{"alias": "joe-bloggs-spain_123", "phoneNumber": "+34644000000"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record CompleteProfileRequest
{
    /// <summary>
    /// The identity alias
    /// </summary>
    /// <remarks>
    /// It must be globally unique and contain lowercase letters, numbers, dashes, and underscores only, between 4 and 24 characters in length
    /// </remarks>
    [Required(ErrorMessage = "alias_required")]
    [MinLength(4, ErrorMessage = "alias_too_short")]
    [MaxLength(24, ErrorMessage = "alias_too_long")]
    [RegularExpression("^[a-z0-9_-]+$", ErrorMessage = "alias_invalid_format")]
    public string Alias { get; init; } = null!;
    
    /// <summary>
    /// The primary contact phone number associated with the individual.
    /// </summary>
    /// <remarks>
    /// Must be provided in E.164 international format (e.g., +34644000000)
    /// </remarks>
    [InternationalPhoneNumber("phone_number_valid_international_format_required")]
    public string PhoneNumber { get; init; } = null!;


}