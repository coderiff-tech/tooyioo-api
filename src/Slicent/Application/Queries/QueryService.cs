using System.Linq.Expressions;
using Eventuous.Projections.MongoDB.Tools;
using MongoDB.Driver;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Slicent.Application.Queries;

public class QueryService<TDocument, TDocumentFilter>
    where TDocument : Document
    where TDocumentFilter : DocumentFilter<TDocument>
{
    protected IMongoCollection<TDocument> Collection { get; }

    public QueryService(IMongoDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        Collection = database.GetDocumentCollection<TDocument>();
    }

    public Task<TDocument?> GetSingleOrDefault(
        string resourceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceId);

        return GetSingleOrDefault(x => x.Id == resourceId, cancellationToken);
    }

    public async Task<TDocument?> GetSingleOrDefault(
        Expression<Func<TDocument, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return await Collection
            .Find(filter)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<TDocument>> Get(
        TDocumentFilter documentFilter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documentFilter);

        var filter = documentFilter.Filter;
        var sortDefinition = GetSortDefinition(documentFilter.Sort);

        var pageNumber = Math.Max(1, documentFilter.PageNumber);
        var pageSize = Math.Clamp(documentFilter.PageSize, 1, 200);
        var skip = (pageNumber - 1) * pageSize;

        const string countFacetName = "count";
        const string dataFacetName = "data";

        var countFacet = AggregateFacet.Create(
            countFacetName,
            PipelineDefinition<TDocument, AggregateCountResult>.Create(
                [PipelineStageDefinitionBuilder.Count<TDocument>()]));

        var dataFacet = AggregateFacet.Create(
            dataFacetName,
            PipelineDefinition<TDocument, TDocument>.Create(
            [
                PipelineStageDefinitionBuilder.Sort(sortDefinition),
                PipelineStageDefinitionBuilder.Skip<TDocument>(skip),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
            ]));

        var aggregation = await Collection
            .Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .FirstOrDefaultAsync(cancellationToken);

        if (aggregation is null)
        {
            return new PagedResult<TDocument>
            {
                Items = [],
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 0,
                TotalCount = 0
            };
        }

        var totalCount = aggregation.Facets
            .First(x => x.Name == countFacetName)
            .Output<AggregateCountResult>()
            .FirstOrDefault()
            ?.Count ?? 0;

        var items = aggregation.Facets
            .First(x => x.Name == dataFacetName)
            .Output<TDocument>();

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResult<TDocument>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }

    private static SortDefinition<TDocument> GetSortDefinition(Sort<TDocument> sort)
    {
        ArgumentNullException.ThrowIfNull(sort);

        var effectiveSort = sort.IsEmpty
            ? Sort<TDocument>.Default
            : sort;

        return effectiveSort.Definitions.Length switch
        {
            0 => Builders<TDocument>.Sort.Ascending(x => x.Id),
            1 => effectiveSort.Definitions[0],
            _ => Builders<TDocument>.Sort.Combine(effectiveSort.Definitions)
        };
    }
}