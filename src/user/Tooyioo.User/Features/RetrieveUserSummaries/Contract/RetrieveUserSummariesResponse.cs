using Slicent.Application.Queries;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.User.Features.RetrieveUserSummaries.Contract;

/// <summary>
/// Response returned after a successful retrieve user summaries request.
/// </summary>
/// <example>
/// {"pageNumber": 1, "pageSize": 20, "totalPages": 1, "totalCount": 1,
/// "items": [{"id": "00000000-0000-0000-0000-000000000001", "alias": "joe-bloggs-spain_123",
/// "name": "Joe", "lastName": "Bloggs"}]}
/// </example>
public sealed record RetrieveUserSummariesResponse
    : PagedResponse<RetrieveUserSummariesResponseItem>;

public sealed record RetrieveUserSummariesResponseItem
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public required string Id { get; init; }
    
    /// <summary>
    /// The public user alias.
    /// </summary>
    public required string Alias { get; init; }
    
    /// <summary>
    /// The user's given name.
    /// </summary>
    public required string Name { get; init; }
    
    /// <summary>
    /// The user's family name.
    /// </summary>
    public required string LastName { get; init; }
}
