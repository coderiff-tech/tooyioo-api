using Slicent.Application.Queries;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.User.Features.Support;

public sealed record UserDocument
    : Document
{
    public UserDocument(string Id) 
        : base(Id)
    {
    }
    
    public required string Alias { get; init; }
    public required string Name { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required bool IsEmailVerified { get; init; }
    public required string PhoneNumber { get; init; }
    public required string ExternalId { get; init; }
    public required string ExternalIdProvider { get; init; }
    public required string UserOnboardingId { get; init; }
    public required string UserOnboardingInitiatedAt { get; init; }
    public required string TermsAndConditionsVersion { get; init; }
}