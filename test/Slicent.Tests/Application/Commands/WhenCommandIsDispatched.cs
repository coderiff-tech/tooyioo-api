using Microsoft.Extensions.DependencyInjection;
using Slicent.Application.Commands;

namespace Slicent.Tests.Application.Commands;

public sealed class WhenCommandIsDispatched
{
    [Test]
    public async Task Then_registered_handler_is_invoked()
    {
        var services = new ServiceCollection();
        services.AddSlicent(typeof(WhenCommandIsDispatched).Assembly);

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        var result = await dispatcher.Send(new TestCommand("Jane"), CancellationToken.None);

        await Assert.That(result.Message).IsEqualTo("Created Jane");
    }

    public sealed record TestCommand(string Name)
        : ICommand<TestCommandResult>;

    public sealed record TestCommandResult(string Message);

    public sealed class TestCommandHandler
        : ICommandHandler<TestCommand, TestCommandResult>
    {
        public Task<TestCommandResult> Handle(
            TestCommand command,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new TestCommandResult($"Created {command.Name}"));
    }
}
