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
    
    public static Task<MultiAppendEventsResult> StoreAggregatesAtomically<
        TAggregate1, TState1, TId1,
        TAggregate2, TState2, TId2>(
        this IMultiAppendEventWriter eventWriter,
        TAggregate1 aggregate1,
        TAggregate2 aggregate2,
        CancellationToken ct = default)
        where TAggregate1 : Aggregate<TState1>
        where TState1 : State<TState1, TId1>, new()
        where TId1 : Id
        where TAggregate2 : Aggregate<TState2>
        where TState2 : State<TState2, TId2>, new()
        where TId2 : Id {

        ArgumentNullException.ThrowIfNull(aggregate1);
        ArgumentNullException.ThrowIfNull(aggregate2);

        if (eventWriter is not IMultiAppendEventWriter multiAppendEventWriter)
        { 
            throw new NotSupportedException("Writer does not support multi-stream append");
        }
        
        var requests = new[]
        {
            ToRequest<TAggregate1,TState1,TId1>(aggregate1),
            ToRequest<TAggregate2,TState2,TId2>(aggregate2)
        };

        return multiAppendEventWriter.AppendEvents(requests, ct);
    }

    private static AppendEventsRequest ToRequest<TAgg,TState,TId>(TAgg aggregate)
        where TAgg : Aggregate<TState>
        where TState : State<TState,TId>, new()
        where TId : Id
    {
        var stream = StreamNameFactory.For<TAgg,TState, TId>(aggregate.State.Id);

        return new AppendEventsRequest(
            stream,
            new ExpectedStreamVersion(aggregate.OriginalVersion),
            aggregate.Changes);
    }
}