using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Slicent.Application.Queries;

internal sealed class QueryDispatcher(IServiceProvider serviceProvider) 
    : IQueryDispatcher
{
    private static readonly ConcurrentDictionary<(Type Query, Type Result), Type> InvokerTypesCache = new();

    public Task<TQueryResult> Send<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var invokerType = InvokerTypesCache.GetOrAdd(
            (query.GetType(), typeof(TQueryResult)),
            crTypes => typeof(QueryInvoker<,>).MakeGenericType(crTypes.Query, crTypes.Result));

        var invoker = (IQueryInvoker<TQueryResult>)serviceProvider.GetRequiredService(invokerType);
        return invoker.Invoke(query, cancellationToken);
    }
}