namespace Slicent.Application.Commands;

internal interface ICommandInvoker<TCommandResult>
{
    Task<TCommandResult> Invoke(ICommand<TCommandResult> command, CancellationToken cancellationToken = default);
}