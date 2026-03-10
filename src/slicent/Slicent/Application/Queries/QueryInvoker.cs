// ReSharper disable ConvertToPrimaryConstructor
namespace Slicent.Application.Queries;

internal class QueryInvoker<TQuery, TQueryResult>
    : IQueryInvoker<TQueryResult> 
    where TQuery : IQuery<TQueryResult>
{
    private readonly IQueryHandler<TQuery, TQueryResult> _handler;

    public QueryInvoker(IQueryHandler<TQuery, TQueryResult> handler)
        => _handler = handler;
    
    public Task<TQueryResult> Invoke(IQuery<TQueryResult> query, CancellationToken cancellationToken = default) 
        => _handler.Handle((TQuery)query, cancellationToken);
}