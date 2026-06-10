// ReSharper disable UnusedParameter.Global

namespace Slicent.Application.Commands;

public interface ICommandMapper<in TRequest, in TContext, out TCommand>
    where TCommand : ICommand
{
    TCommand Map(TRequest request, TContext context);
}

public interface ICommandMapper<in TParams, in TRequest, in TContext, out TCommand>
    where TCommand : ICommand
{
    TCommand Map(TParams urlParams, TRequest request, TContext context);
}