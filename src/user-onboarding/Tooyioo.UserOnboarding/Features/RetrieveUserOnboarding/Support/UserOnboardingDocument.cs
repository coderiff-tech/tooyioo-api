using Slicent.Application.Queries;

// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding.Support;

public sealed record UserOnboardingDocument
    : Document
{
    public UserOnboardingDocument(string Id) 
        : base(Id)
    {
    }

    public required string Name { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public bool IsEmailVerified { get; init; }
    public required ExternalIdentityDocument ExternalIdentity { get; init; }
    public string? Alias { get; init; }
    public required DateTime? CompletedAt { get; init; }
}

public sealed record ExternalIdentityDocument
{
    public required string Id { get; init; }
    public required string Provider { get; init; }
    public required string Issuer { get; init; }
}