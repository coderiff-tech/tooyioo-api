// ReSharper disable ClassNeverInstantiated.Global

using Microsoft.AspNetCore.Mvc;

namespace Tooyioo.User.Features.RetrieveUserSummaries.Contract;

public record RetrieveUserSummariesRoute
{
    [FromQuery]
    public string? Id { get; init; }

    [FromQuery]
    public string? Alias { get; init; }

    [FromQuery]
    public int? PageNumber { get; init; }

    [FromQuery]
    public int? PageSize { get; init; }
}
