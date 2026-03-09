using Eventuous;

namespace Slicent.EventStore;

public sealed record MultiAppendEventsResult(
    ulong GlobalPosition,
    IReadOnlyCollection<StreamAppendResult> StreamAppendResults)
{
    public StreamAppendResult? For(StreamName stream)
        => StreamAppendResults.FirstOrDefault(x => x.Stream == stream);
}

public sealed record StreamAppendResult(StreamName Stream, long NextExpectedVersion);
