// ReSharper disable UnusedParameter.Global
namespace Slicent.Application.Commands;

public interface ICommandHandler<in TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    Task<TCommandResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
