// ReSharper disable ConvertToPrimaryConstructor
namespace Slicent.Application.Commands;

internal class CommandInvoker<TCommand, TCommandResult>
    : ICommandInvoker<TCommandResult> 
    where TCommand : ICommand<TCommandResult>
{
    private readonly ICommandHandler<TCommand, TCommandResult> _handler;

    public CommandInvoker(ICommandHandler<TCommand, TCommandResult> handler)
        => _handler = handler;
    
    public Task<TCommandResult> Invoke(ICommand<TCommandResult> command, CancellationToken cancellationToken = default) 
        => _handler.Handle((TCommand)command, cancellationToken);
}