namespace Slicent.Application.Queries;

public interface IQueryDispatcher
{
    Task<TQueryResult> Send<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken);
}