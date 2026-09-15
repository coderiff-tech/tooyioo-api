using System.ComponentModel.DataAnnotations;

namespace Tooyioo.Api.Infrastructure.OpenApi;

public sealed class ScalarGoogleOAuthOptions
{
    public const string SectionName = "Scalar:GoogleOAuth";

    [Required]
    public string ClientId { get; init; } = string.Empty;

    public string? ClientSecret { get; init; }

    [Required]
    [Url]
    public string AuthorizationUrl { get; init; } = string.Empty;

    [Required]
    [Url]
    public string TokenUrl { get; init; } = string.Empty;
}
