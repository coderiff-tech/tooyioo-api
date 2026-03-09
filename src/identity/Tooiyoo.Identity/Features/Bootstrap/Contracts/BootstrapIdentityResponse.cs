// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Tooiyoo.Identity.Features.Bootstrap.Contracts;

/// <summary>
/// Response returned after a successful identity bootstrap request
/// </summary>
/// <remarks>
/// It contains the unique identifier of the identity
/// </remarks>
/// <example>{"id": "00000000-0000-0000-0000-000000000001", "externalId": "12345", "externalIdentityProvider": "Google"}</example>
public sealed record BootstrapIdentityResponse
{
    /// <summary>
    /// The unique identifier of the created identity
    /// </summary>
    /// <remarks>
    /// This identifier can be used to reference the identity in later operations
    /// </remarks>
    public required string Id { get; init; }
    
    /// <summary>
    /// The external identifier of the created identity
    /// </summary>
    /// <remarks>
    /// This identifier is unique within the external identity provider
    /// </remarks>
    public required string ExternalId { get; init; }

    /// <summary>
    /// The external identity provider for the created identity
    /// </summary>
    public required string ExternalIdentityProvider { get; init; }
}