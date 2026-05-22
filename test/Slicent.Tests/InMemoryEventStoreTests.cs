using Eventuous;
using Slicent.EventStore;

namespace Slicent.Tests;

public sealed class InMemoryEventStoreTests
{
    [Test]
    public async Task ReadEventsReturnsSnapshotFromStart()
    {
        var store = new InMemoryEventStore();
        var stream = new StreamName($"test-{Guid.NewGuid():N}");

        await store.AppendEvents(
            stream,
            ExpectedStreamVersion.NoStream,
            [
                CreateEvent(1),
                CreateEvent(2),
                CreateEvent(3)
            ]);

        var events = await Collect(store.ReadEvents(stream, StreamReadPosition.Start, 2, CancellationToken.None));

        if (events.Length != 2)
        {
            throw new InvalidOperationException($"Expected 2 events, got {events.Length}");
        }

        AssertEvent(events[0], 1, 0);
        AssertEvent(events[1], 2, 1);
    }

    [Test]
    public async Task ReadEventsBackwardsHandlesEndAndBounds()
    {
        var store = new InMemoryEventStore();
        var stream = new StreamName($"test-{Guid.NewGuid():N}");

        await store.AppendEvents(
            stream,
            ExpectedStreamVersion.NoStream,
            [
                CreateEvent(1),
                CreateEvent(2),
                CreateEvent(3)
            ]);

        var fromEnd = await Collect(store.ReadEventsBackwards(stream, StreamReadPosition.End, 2, CancellationToken.None));
        var beyondEnd = await Collect(store.ReadEventsBackwards(stream, new StreamReadPosition(100), 10, CancellationToken.None));

        if (fromEnd.Length != 2)
        {
            throw new InvalidOperationException($"Expected 2 events from end, got {fromEnd.Length}");
        }

        AssertEvent(fromEnd[0], 3, 2);
        AssertEvent(fromEnd[1], 2, 1);

        if (beyondEnd.Length != 3)
        {
            throw new InvalidOperationException($"Expected 3 events from beyond end, got {beyondEnd.Length}");
        }

        AssertEvent(beyondEnd[0], 3, 2);
        AssertEvent(beyondEnd[1], 2, 1);
        AssertEvent(beyondEnd[2], 1, 0);
    }

    [Test]
    public async Task MissingStreamsThrowStreamNotFound()
    {
        var store = new InMemoryEventStore();
        var stream = new StreamName($"missing-{Guid.NewGuid():N}");

        try
        {
            _ = await Collect(store.ReadEvents(stream, StreamReadPosition.Start, 1, CancellationToken.None));
        }
        catch (StreamNotFound)
        {
            return;
        }

        throw new InvalidOperationException("Expected StreamNotFound");
    }

    private static NewStreamEvent CreateEvent(int value)
        => new(Guid.NewGuid(), new TestEvent(value), new Metadata());

    private static async Task<StreamEvent[]> Collect(IAsyncEnumerable<StreamEvent> events)
    {
        var result = new List<StreamEvent>();

        await foreach (var streamEvent in events)
        {
            result.Add(streamEvent);
        }

        return result.ToArray();
    }

    private static void AssertEvent(StreamEvent streamEvent, int value, long revision)
    {
        if (streamEvent.Payload is not TestEvent testEvent || testEvent.Value != value)
        {
            throw new InvalidOperationException($"Expected payload value {value}");
        }

        if (streamEvent.Revision != revision)
        {
            throw new InvalidOperationException($"Expected revision {revision}, got {streamEvent.Revision}");
        }
    }

    private sealed record TestEvent(int Value);
}
