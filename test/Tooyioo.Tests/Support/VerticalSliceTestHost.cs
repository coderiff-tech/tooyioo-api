using Eventuous;
using Eventuous.Testing;
using Microsoft.Extensions.DependencyInjection;
using Slicent.EventStore;
using Tooyioo.Tests.Support.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Tooyioo.Tests.Support;

public sealed class VerticalSliceTestHost
    : IAsyncDisposable
{
    private readonly VerticalSliceWebApplicationFactory _factory;
    private readonly byte[] _signingKey;

    private VerticalSliceTestHost(
        VerticalSliceWebApplicationFactory factory,
        HttpClient httpClient,
        InMemoryEventStore eventStore,
        byte[] signingKey)
    {
        _factory = factory;
        _signingKey = signingKey;
        HttpClient = httpClient;
        EventStore = eventStore;
        Events = new EventAssertions(eventStore);
    }

    public HttpClient HttpClient { get; }
    public InMemoryEventStore EventStore { get; }
    public EventAssertions Events { get; }

    public static Task<VerticalSliceTestHost> Start(Action<IServiceCollection>? overrideServices = null)
    {
        var eventStore = new InMemoryEventStore();
        var signingKey = GoogleIdentityTokenBuilder.CreateSigningKey();
        var factory = new VerticalSliceWebApplicationFactory(eventStore, signingKey, overrideServices);
        var client = factory.CreateClient();

        return Task.FromResult(new VerticalSliceTestHost(factory, client, eventStore, signingKey));
    }

    public GoogleIdentityTokenBuilder GoogleIdentityToken()
        => new(_signingKey);

    public async Task Given<TState, TId>(TId id, params object[] events)
        where TState : State<TState, TId>, new()
        where TId : Id
    {
        var stream = await EventStore.LoadStateOrNew<TState, TId>(id, CancellationToken.None);
        await EventStore.StoreStateChanges([stream.ToStateStreamChanges(events)], CancellationToken.None);
    }

    public async ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        await _factory.DisposeAsync();
    }
}
