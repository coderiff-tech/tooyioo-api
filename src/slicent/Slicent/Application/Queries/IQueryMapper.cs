// ReSharper disable UnusedParameter.Global
namespace Slicent.Application.Queries;

public interface IQueryMapper<in TContext, out TQuery>
    where TQuery : IQuery
{
    TQuery Map(TContext context);
}

public interface IQueryMapper<in TParams, in TContext, out TQuery>
    where TQuery : IQuery
{
    TQuery Map(TParams urlParams, TContext context);
}