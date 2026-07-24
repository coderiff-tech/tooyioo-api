namespace Slicent.Application.Queries;

public abstract record PagedResponse<TItem>
{
    /// <summary>
    /// The current one-based page number returned by the query.
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// The maximum number of items requested for this page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// The total number of pages available for the query.
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    /// The total number of items matching the query.
    /// </summary>
    public required long TotalCount { get; init; }

    /// <summary>
    /// The items returned for the current page.
    /// </summary>
    public required IEnumerable<TItem> Items { get; init; }
}
