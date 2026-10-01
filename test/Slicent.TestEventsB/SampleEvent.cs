using Slicent.EventStore;

namespace Slicent.TestEventsB;

[DomainEventType("Test.SharedName")]
public sealed record SampleEvent(int Number);
