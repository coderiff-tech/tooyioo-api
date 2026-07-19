using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

namespace Tooyioo.UserOnboarding.Features.CancelUserOnboarding.Contracts;

/// <summary>
/// Request payload used to cancel user onboarding
/// </summary>
/// <remarks>
/// It's idempotent. If the onboarding is already canceled, it will be returned as-is
/// </remarks>
/// <example>{"Reason": "Terms and Conditions rejected"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record CancelUserOnboardingRequest
{
    /// <summary>
    /// The reason for the cancellation
    /// </summary>
    [Required(ErrorMessage = "reason_required")]
    public string Reason { get; init; } = string.Empty;
}