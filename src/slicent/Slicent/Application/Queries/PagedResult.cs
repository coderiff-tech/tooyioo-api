using Eventuous.Projections.MongoDB.Tools;

namespace Slicent.Application.Queries;

public record PagedResult<TDocument>
    where TDocument : ProjectedDocument
{
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalPages { get; init; }
    public required long TotalCount { get; init; }
    public required IEnumerable<TDocument> Items { get; init; } = [];
}