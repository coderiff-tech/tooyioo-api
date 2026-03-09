using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

namespace Tooiyoo.Identity.Features.Bootstrap.Contracts;

/// <summary>
/// Request payload used to bootstrap an identity out of an external subject (e.g., a Google sub)
/// </summary>
/// <remarks>
/// It's idempotent. If the identity already exists, it will be returned as-is
/// </remarks>
/// <example>{"externalIdentityProvider": "Google"}</example>
#pragma warning disable ASP0029
[ValidatableType]
#pragma warning restore ASP0029
public sealed record BootstrapIdentityRequest
{
    /// <summary>
    /// The external identity provider. Allowed values are: Google
    /// </summary>
    [Required(ErrorMessage = "external_identity_provided_required")]
    [AllowedValues("Google", ErrorMessage = "external_identity_provider_invalid")]
    public string ExternalIdentityProvider { get; init; } = null!;
}