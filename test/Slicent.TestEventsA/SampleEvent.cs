using Slicent.EventStore;

namespace Slicent.TestEventsA;

[DomainEventType("Test.SharedName")]
public sealed record SampleEvent(string Value);
