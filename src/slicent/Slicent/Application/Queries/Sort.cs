using System.Collections.Immutable;
using System.Linq.Expressions;
using MongoDB.Driver;

// ReSharper disable MemberCanBePrivate.Global

namespace Slicent.Application.Queries;

public sealed record Sort<TDocument>
    where TDocument : Document
{
    public static Sort<TDocument> Default { get; } =
        new Sort<TDocument>().AddAscendingExpression(x => x.Id);

    public ImmutableArray<SortDefinition<TDocument>> Definitions { get; init; } = ImmutableArray<SortDefinition<TDocument>>.Empty;

    public bool IsEmpty => Definitions.IsDefaultOrEmpty;

    public Sort<TDocument> AddAscendingExpression(Expression<Func<TDocument, object>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return this with
        {
            Definitions = Definitions.Add(Builders<TDocument>.Sort.Ascending(expression))
        };
    }

    public Sort<TDocument> AddDescendingExpression(Expression<Func<TDocument, object>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return this with
        {
            Definitions = Definitions.Add(Builders<TDocument>.Sort.Descending(expression))
        };
    }
}