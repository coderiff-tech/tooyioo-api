// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Tooyioo.Profile.Features.Bootstrap.Contracts;

/// <summary>
/// Response returned after a successful profile bootstrap request
/// </summary>
/// <remarks>
/// It contains the unique identifier of the profile
/// </remarks>
/// <example>{"id": "00000000-0000-0000-0000-000000000001", "externalId": "12345", "externalIdProvider": "Google"}</example>
public sealed record BootstrapProfileResponse
{
    /// <summary>
    /// The unique identifier of the created profile
    /// </summary>
    /// <remarks>
    /// This identifier can be used to reference the profile in later operations
    /// </remarks>
    public required string Id { get; init; }
    
    /// <summary>
    /// The external identifier of the created profile
    /// </summary>
    /// <remarks>
    /// This identifier is unique within the external provider
    /// </remarks>
    public required string ExternalId { get; init; }

    /// <summary>
    /// The external identity provider for the created profile
    /// </summary>
    public required string ExternalIdProvider { get; init; }
}