// using System.ComponentModel.DataAnnotations;
// using Microsoft.Extensions.Validation;
//
// namespace Tooyioo.Profile.Features.Complete.Contracts;
//
// /// <summary>
// /// Request payload used to complete the profile
// /// </summary>
// /// <example>{"alias": "joe-bloggs-spain_123"}</example>
// #pragma warning disable ASP0029
// [ValidatableType]
// #pragma warning restore ASP0029
// public sealed record CompleteProfileRequest
// {
//     /// <summary>
//     /// The identity alias
//     /// </summary>
//     /// <remarks>
//     /// It must be globally unique and contain lowercase letters, numbers, dashes, and underscores only, between 4 and 24 characters in length
//     /// </remarks>
//     [Required(ErrorMessage = "alias_required")]
//     [MinLength(4, ErrorMessage = "alias_too_short")]
//     [MaxLength(24, ErrorMessage = "alias_too_long")]
//     [RegularExpression("^[a-zA-Z0-9_-]+$", ErrorMessage = "alias_invalid_format")]
//     public string Alias { get; init; } = null!;
// }