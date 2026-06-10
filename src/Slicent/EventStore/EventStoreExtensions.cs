using Eventuous;

// ReSharper disable ConvertToExtensionBlock
// ReSharper disable InvertIf

namespace Slicent.EventStore;

public static class EventStoreExtensions
{
    public static async Task<TAggregate> LoadAggregateOrNew<TAggregate, TState, TId>(
        this IEventReader eventReader,
        TId id,
        CancellationToken ct)
        where TAggregate : Aggregate<TState>, new()
        where TState : State<TState>, new()
        where TId : Id =>
        await eventReader.LoadAggregate<TAggregate, TState, TId>(id, failIfNotFound: false, cancellationToken: ct);

    public static Task<AppendEventsResult[]> Store<
        TAggregate1, TState1, TId1,
        TAggregate2, TState2, TId2>(
        this IEventWriter eventWriter,
        TAggregate1 aggregate1,
        TAggregate2 aggregate2,
        CancellationToken ct = default)
        where TAggregate1 : Aggregate<TState1>
        where TState1 : State<TState1, TId1>, new()
        where TId1 : Id
        where TAggregate2 : Aggregate<TState2>
        where TState2 : State<TState2, TId2>, new()
        where TId2 : Id
    {
        ArgumentNullException.ThrowIfNull(eventWriter);
        ArgumentNullException.ThrowIfNull(aggregate1);
        ArgumentNullException.ThrowIfNull(aggregate2);

        var streams =
            new List<(StreamName StreamName, ExpectedStreamVersion ExpectedVersion, IReadOnlyCollection<object> Events
                )>();

        if (aggregate1.Changes.Count > 0)
        {
            var streamOne = StreamNameFactory.For<TAggregate1, TState1, TId1>(aggregate1.State.Id);

            streams.Add((
                streamOne,
                new ExpectedStreamVersion(aggregate1.OriginalVersion),
                aggregate1.Changes
            ));
        }

        if (aggregate2.Changes.Count > 0)
        {
            var streamTwo = StreamNameFactory.For<TAggregate2, TState2, TId2>(aggregate2.State.Id);

            streams.Add((
                streamTwo,
                new ExpectedStreamVersion(aggregate2.OriginalVersion),
                aggregate2.Changes
            ));
        }

        return streams.Count == 0
            ? Task.FromResult(Array.Empty<AppendEventsResult>())
            : eventWriter.Store(streams, cancellationToken: ct);
    }
}