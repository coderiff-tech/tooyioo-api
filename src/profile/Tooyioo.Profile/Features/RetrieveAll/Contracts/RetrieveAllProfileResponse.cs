// ReSharper disable ClassNeverInstantiated.Global
namespace Tooyioo.Profile.Features.RetrieveAll.Contracts;

/// <summary>
/// Response returned after a successful retrieve all profile
/// </summary>
/// <example>
/// {"pageNumber": 1, "pageSize": 20, "totalPages": 1, "totalCount": 1,
/// "items": [{"id": "00000000-0000-0000-0000-000000000001", "alias": "joe-bloggs-spain_123",
/// "isComplete": true, "createdAt": "2026-01-01T00:00:00.123Z", "lastModifiedAt": "2026-01-01T00:00:00.123Z",
/// "completedAt": "2026-01-01T00:00:00.123Z"}]}
/// </example>
public sealed record RetrieveAllProfileResponse
{
    /// <summary>
    /// The current page number
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// The requested page size
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// The total number of pages available
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    /// The total number of items available
    /// </summary>
    public required long TotalCount { get; init; }

    /// <summary>
    /// The items
    /// </summary>
    public required IEnumerable<RetrieveAllProfileResponseItem> Items { get; init; }
}

public sealed record RetrieveAllProfileResponseItem
{
    /// <summary>
    /// The unique identifier of the profile
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The unique alias (if any)
    /// </summary>
    public required string Alias { get; init; }
    
    /// <summary>
    /// Flag indicating whether the profile is complete or needs to be completed
    /// </summary>
    public required bool IsComplete { get; init; }
    
    /// <summary>
    /// Creation date
    /// </summary>
    public required DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Last modification date
    /// </summary>
    public required DateTime LastModifiedAt { get; init; }
    
    /// <summary>
    /// Profile completion date (if any)
    /// </summary>
    public required DateTime? CompletedAt { get; init; }
}
