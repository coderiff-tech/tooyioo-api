using Eventuous;

namespace Slicent.EventStore;

public interface IMultiAppendEventWriter 
    : IEventWriter 
{
    Task<MultiAppendEventsResult> AppendEvents(
        IReadOnlyCollection<AppendEventsRequest> appendEventsRequests,
        CancellationToken cancellationToken = default);
}

public readonly record struct AppendEventsRequest(
    StreamName StreamName,
    ExpectedStreamVersion ExpectedVersion,
    IReadOnlyCollection<object> Changes,
    AmendEvent? AmendEvent = null
);