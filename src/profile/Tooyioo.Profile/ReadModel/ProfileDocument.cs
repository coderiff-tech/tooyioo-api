using Eventuous.Projections.MongoDB.Tools;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.ReadModel;

public sealed record ProfileDocument
    : ProjectedDocument
{
    public ProfileDocument(string Id) 
        : base(Id)
    {
    }

    public required string ExternalId { get; init; }
    public required string ExternalProviderName { get; init; }
    public required string Name { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required bool IsEmailVerified { get; init; }
    public required string AuthenticationProvider { get; init; }
    public required string Alias { get; init; }
    public bool IsProfileComplete { get; init; }
}