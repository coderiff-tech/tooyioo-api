using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

namespace Tooyioo.UserOnboarding.Features.CompleteUserOnboarding.Contracts;

/// <summary>
/// Request payload used to complete user onboarding
/// </summary>
/// <remarks>
/// It's idempotent. If the onboarding is already completed, it will be returned as-is
/// </remarks>
/// <example>{"TermsAndConditionsVersion": "v1"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record CompleteUserOnboardingRequest
{
    /// <summary>
    /// The version of the terms and conditions
    /// </summary>
    [Required(ErrorMessage = "terms_and_conditions_version_required")]
    public string TermsAndConditionsVersion { get; init; } = string.Empty;
}