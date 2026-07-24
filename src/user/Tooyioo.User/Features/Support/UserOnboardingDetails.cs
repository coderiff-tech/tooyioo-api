namespace Tooyioo.User.Features.Support;

public sealed record UserOnboardingDetails
{
    public required string Id { get; init; }
    public required string Alias { get; init; }
    public required string Name { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required bool IsEmailVerified { get; init; }
    public required string ExternalId { get; init; }
    public required string ExternalProvider { get; init; }
}