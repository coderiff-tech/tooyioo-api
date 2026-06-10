// ReSharper disable UnusedTypeParameter
namespace Slicent.Application.Queries;

public interface IQuery;

public interface IQuery<out TQueryResult> 
    : IQuery;