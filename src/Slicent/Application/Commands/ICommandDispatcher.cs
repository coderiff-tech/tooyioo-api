namespace Slicent.Application.Commands;

public interface ICommandDispatcher
{
    Task<TCommandResult> Send<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken);
}