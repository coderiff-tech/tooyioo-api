using Slicent.Application.Queries;

namespace Slicent.Tests.Support;

public sealed record TestDocument(string Id)
    : Document(Id)
{
    public required string Alias { get; init; }
    public required string Name { get; init; }
    public required int Score { get; init; }
}
