namespace Slicent.Application.Queries;

internal interface IQueryInvoker<TQueryResult>
{
    Task<TQueryResult> Invoke(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
}