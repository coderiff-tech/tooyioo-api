using Eventuous.Projections.MongoDB.Tools;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.ReadModel;

public sealed record ProfileDocument
    : ProjectedDocument
{
    public ProfileDocument(string Id) 
        : base(Id)
    {
    }

    public required string Name { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public string Alias { get; init; } = string.Empty;
}