using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Contracts;

/// <summary>
/// Request payload used to initiate onboarding out of an external subject (e.g., a Google sub)
/// </summary>
/// <remarks>
/// It's idempotent. If the onboarding already happened, it will be returned as-is
/// </remarks>
/// <example>{"externalIdProvider": "Google"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record InitiateUserOnboardingRequest
{
    /// <summary>
    /// The external Id provider. Allowed values are: Google
    /// </summary>
    [Required(ErrorMessage = "external_id_provided_required")]
    [AllowedValues("Google", ErrorMessage = "external_id_provider_invalid")]
    public string ExternalIdProvider { get; init; } = null!;
}