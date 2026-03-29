using Microsoft.Extensions.Validation;
using Tooyioo.Profile.Features.SetPhoneNumber.Support;

namespace Tooyioo.Profile.Features.SetPhoneNumber.Contracts;

/// <summary>
/// Request payload used to set the profile phone number
/// </summary>
/// <example>{"phoneNumber": "+34644000000"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record SetProfilePhoneNumberRequest
{
    /// <summary>
    /// The optional primary contact phone number associated with the individual.
    /// </summary>
    /// <remarks>
    /// Must be provided in E.164 international format (e.g., +34644000000)
    /// </remarks>
    [InternationalPhoneNumber("phone_number_valid_international_format_required")]
    public string? PhoneNumber { get; init; } = null!;
}