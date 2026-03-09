using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Slicent.Application.Commands;

internal sealed class CommandDispatcher(IServiceProvider serviceProvider) 
    : ICommandDispatcher
{
    private static readonly ConcurrentDictionary<(Type Command, Type Result), Type> InvokerTypesCache = new();

    public Task<TCommandResult> Send<TCommandResult>(ICommand<TCommandResult> command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var invokerType = InvokerTypesCache.GetOrAdd(
            (command.GetType(), typeof(TCommandResult)),
            crTypes => typeof(CommandInvoker<,>).MakeGenericType(crTypes.Command, crTypes.Result));

        var invoker = (ICommandInvoker<TCommandResult>)serviceProvider.GetRequiredService(invokerType);
        return invoker.Invoke(command, cancellationToken);
    }
}