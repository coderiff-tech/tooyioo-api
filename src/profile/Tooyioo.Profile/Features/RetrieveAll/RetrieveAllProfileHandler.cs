using System.Linq.Expressions;
using Slicent;
using Slicent.Application.Queries;
using Tooyioo.Profile.ReadModel;
// ReSharper disable InvertIf
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.RetrieveAll;

public sealed class RetrieveAllProfileHandler
    : IQueryHandler<RetrieveAllProfileQuery, RetrieveAllProfileQueryResult>
{
    private readonly QueryService<ProfileDocument, RetrieveAllProfileQuery> _queryService;

    public RetrieveAllProfileHandler(QueryService<ProfileDocument, RetrieveAllProfileQuery> queryService)
    {
        _queryService = queryService;
    }

    public async Task<RetrieveAllProfileQueryResult> Handle(
        RetrieveAllProfileQuery query, 
        CancellationToken cancellationToken = default)
    {
        var pagedResult = await _queryService.Get(query, cancellationToken);
        return new RetrieveAllProfileQueryResult(pagedResult);
    }
}

public sealed record RetrieveAllProfileQuery
    : DocumentFilter<ProfileDocument>, IQuery<RetrieveAllProfileQueryResult>
{
    public string? Alias { get; init; }
    public bool? IsComplete { get; init; }
    public DateTime? CreatedAtFrom { get; init; }
    public DateTime? CreatedAtTo { get; init; }
    public DateTime? CompletedAtFrom { get; init; }
    public DateTime? CompletedAtTo { get; init; }
    public DateTime? LastModifiedAtFrom { get; init; }
    public DateTime? LastModifiedAtTo { get; init; }

    public override Expression<Func<ProfileDocument, bool>> Filter
    {
        get
        {
            var filter = base.Filter;

            if (Alias is not null)
            {
                filter = filter.AndAlso(x => x.Alias.Contains(Alias));
            }
            
            if (IsComplete is not null)
            {
                filter = filter.AndAlso(x => x.IsComplete == IsComplete);
            }
            
            if (CreatedAtFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(CreatedAtFrom.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.CreatedAt >= from);
            }

            if (CreatedAtTo.HasValue)
            {
                var to = DateTime.SpecifyKind(CreatedAtTo.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.CreatedAt <= to);
            }
            
            if (CompletedAtFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(CompletedAtFrom.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.CompletedAt >= from);
            }

            if (CompletedAtTo.HasValue)
            {
                var to = DateTime.SpecifyKind(CompletedAtTo.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.CompletedAt <= to);
            }
            
            if (LastModifiedAtFrom.HasValue)
            {
                var from = DateTime.SpecifyKind(LastModifiedAtFrom.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.LastModifiedAt >= from);
            }

            if (LastModifiedAtTo.HasValue)
            {
                var to = DateTime.SpecifyKind(LastModifiedAtTo.Value, DateTimeKind.Utc);
                filter = filter.AndAlso(x => x.LastModifiedAt <= to);
            }

            return filter;
        }
    }
}

public sealed record RetrieveAllProfileQueryResult(PagedResult<ProfileDocument> PagedResult);
