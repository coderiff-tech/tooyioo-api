using Funzo;
using Slicent.Application.Queries;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.ReadModel;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Retrieve;

public sealed class RetrieveProfileHandler
    : IQueryHandler<RetrieveProfileQuery, RetrieveProfileQueryResult>
{
    private readonly QueryService<ProfileDocument, RetrieveProfileQuery> _queryService;

    public RetrieveProfileHandler(QueryService<ProfileDocument, RetrieveProfileQuery> queryService)
    {
        _queryService = queryService;
    }

    public async Task<RetrieveProfileQueryResult> Handle(
        RetrieveProfileQuery query, 
        CancellationToken cancellationToken = default)
    {
        var queryResult = await _queryService.GetSingleOrDefault(query.ProfileId, cancellationToken);
        if (queryResult is null)
        {
            return new RetrieveProfileQueryNotFoundResult(query.ProfileId);
        }
        return new RetrieveProfileQueryOkResult(queryResult);
    }
}

public sealed record RetrieveProfileQuery(ProfileId ProfileId)
    : DocumentFilter<ProfileDocument>, IQuery<RetrieveProfileQueryResult>;

[Result<RetrieveProfileQueryOkResult, RetrieveProfileQueryNotFoundResult>]
public partial class RetrieveProfileQueryResult;

public sealed record RetrieveProfileQueryOkResult(ProfileDocument Document);
public sealed record RetrieveProfileQueryNotFoundResult(ProfileId Id);