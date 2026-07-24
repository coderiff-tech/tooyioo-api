using Microsoft.Extensions.DependencyInjection;
using Slicent.Application.Queries;

namespace Slicent.Tests.Application.Queries;

public sealed class WhenQueryIsDispatched
{
    [Test]
    public async Task Then_registered_handler_is_invoked()
    {
        var services = new ServiceCollection();
        services.AddSlicent(typeof(WhenQueryIsDispatched).Assembly);

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IQueryDispatcher>();

        var result = await dispatcher.Send(new TestQuery("Jane"), CancellationToken.None);

        await Assert.That(result.Message).IsEqualTo("Hello Jane");
    }

    public sealed record TestQuery(string Name)
        : IQuery<TestQueryResult>;

    public sealed record TestQueryResult(string Message);

    public sealed class TestQueryHandler
        : IQueryHandler<TestQuery, TestQueryResult>
    {
        public Task<TestQueryResult> Handle(
            TestQuery query,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new TestQueryResult($"Hello {query.Name}"));
    }
}
