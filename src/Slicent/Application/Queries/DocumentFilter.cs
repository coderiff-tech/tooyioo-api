using System.Linq.Expressions;

namespace Slicent.Application.Queries;

public abstract record DocumentFilter<TDocument>
    where TDocument : Document
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IsDescending { get; init; }
    public string? SortBy { get; init; }

    public virtual Expression<Func<TDocument, bool>> Filter => x => true;

    public virtual Sort<TDocument> Sort => Sort<TDocument>.Default;

    protected static bool IsSortRequired(string? sortBy, string fieldName) =>
        string.Equals(sortBy, fieldName, StringComparison.OrdinalIgnoreCase);

    protected static Sort<TDocument> GetSortingExpression(
        bool isDescending,
        Expression<Func<TDocument, object>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return isDescending
            ? new Sort<TDocument>().AddDescendingExpression(expression)
            : new Sort<TDocument>().AddAscendingExpression(expression);
    }
}