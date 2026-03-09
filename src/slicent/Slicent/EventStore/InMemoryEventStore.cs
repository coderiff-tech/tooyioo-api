using System.Collections.Concurrent;
using Eventuous;
// ReSharper disable ConvertToPrimaryConstructor

namespace Slicent.EventStore;

public sealed class InMemoryEventStore
    : IEventStore, IMultiAppendEventWriter
{
    private readonly Lock _gate = new();
    private readonly ConcurrentDictionary<StreamName, InMemoryStream> _storage = new();
    private readonly List<StreamEvent> _global = [];

    public Task<bool> StreamExists(StreamName streamName, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            return Task.FromResult(_storage.ContainsKey(streamName));
        }
    }

    public Task<AppendEventsResult> AppendEvents(
        StreamName stream,
        ExpectedStreamVersion expectedVersion,
        IReadOnlyCollection<NewStreamEvent> events,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(events);

        lock (_gate)
        {
            var existing = _storage.GetOrAdd(stream, s => new InMemoryStream(s));

            if (events.Count == 0)
            {
                var pos = _global.Count == 0
                    ? 0UL
                    : (ulong)(_global.Count - 1);

                return Task.FromResult(new AppendEventsResult(pos, existing.Version));
            }

            existing.AppendEvents(expectedVersion, events);

            var globalStart = _global.Count;

            for (var i = 0; i < events.Count; i++)
            {
                var e = events.ElementAt(i);

                _global.Add(
                    new StreamEvent(
                        e.Id,
                        e.Payload,
                        e.Metadata,
                        "application/json",
                        globalStart + i
                    )
                );
            }

            return Task.FromResult(
                new AppendEventsResult(
                    (ulong)(_global.Count - 1),
                    existing.Version
                )
            );
        }
    }

    public Task<StreamEvent[]> ReadEvents(
        StreamName stream,
        StreamReadPosition start,
        int count,
        bool failIfNotFound,
        CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            return Task.FromResult(
                FindStream(stream, failIfNotFound)
                    .GetEvents(start, count)
                    .ToArray()
            );
        }
    }

    public Task<StreamEvent[]> ReadEventsBackwards(
        StreamName stream,
        StreamReadPosition start,
        int count,
        bool failIfNotFound,
        CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            return Task.FromResult(
                FindStream(stream, failIfNotFound)
                    .GetEventsBackwards(start, count)
                    .ToArray()
            );
        }
    }

    public Task TruncateStream(
        StreamName stream,
        StreamTruncatePosition truncatePosition,
        ExpectedStreamVersion expectedVersion,
        CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            FindStream(stream, expectedVersion.ExistingStream).Truncate(expectedVersion, truncatePosition);
            return Task.CompletedTask;
        }
    }

    public Task DeleteStream(
        StreamName stream,
        ExpectedStreamVersion expectedVersion,
        CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            var existing = FindStream(stream, expectedVersion.ExistingStream);
            existing.CheckVersion(expectedVersion);

            _storage.TryRemove(stream, out _);

            return Task.CompletedTask;
        }
    }

    public Task<MultiAppendEventsResult> AppendEvents(
        IReadOnlyCollection<AppendEventsRequest> appendEventsRequests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(appendEventsRequests);

        var reqs = appendEventsRequests
            .Where(r => r.Changes is { Count: > 0 })
            .ToArray();

        lock (_gate)
        {
            if (reqs.Length == 0)
            {
                var pos = _global.Count == 0
                    ? 0UL
                    : (ulong)(_global.Count - 1);

                return Task.FromResult(
                    new MultiAppendEventsResult(
                        pos,
                        Array.Empty<StreamAppendResult>()
                    )
                );
            }

            var duplicate = reqs
                .GroupBy(r => r.StreamName)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicate is not null)
            {
                throw new ArgumentException(
                    $"Same stream appears multiple times in one append: {duplicate.Key}",
                    nameof(appendEventsRequests)
                );
            }

            var resolved = new Dictionary<StreamName, InMemoryStream>(reqs.Length);

            foreach (var req in reqs)
            {
                var stream = _storage.GetOrAdd(req.StreamName, s => new InMemoryStream(s));
                resolved.Add(req.StreamName, stream);
            }

            foreach (var req in reqs)
            {
                resolved[req.StreamName].CheckVersion(req.ExpectedVersion);
            }

            var appended = new List<NewStreamEvent>();

            foreach (var req in reqs)
            {
                var streamEvents = req.Changes
                    .Select(payload =>
                    {
                        var se = new NewStreamEvent(Guid.NewGuid(), payload, new Metadata());

                        if (req.AmendEvent is not null)
                        {
                            se = req.AmendEvent(se);
                        }

                        return se;
                    })
                    .ToArray();

                resolved[req.StreamName].AppendEventsUnchecked(streamEvents);
                appended.AddRange(streamEvents);
            }

            var globalStart = _global.Count;

            for (var i = 0; i < appended.Count; i++)
            {
                var e = appended[i];

                _global.Add(
                    new StreamEvent(
                        e.Id,
                        e.Payload,
                        e.Metadata,
                        "application/json",
                        globalStart + i
                    )
                );
            }

            var globalPosition = _global.Count == 0
                ? 0UL
                : (ulong)(_global.Count - 1);

            var responses = reqs
                .Select(req =>
                {
                    var stream = resolved[req.StreamName];

                    return new StreamAppendResult(
                        req.StreamName,
                        stream.Version
                    );
                })
                .ToArray();

            return Task.FromResult(
                new MultiAppendEventsResult(
                    globalPosition,
                    responses
                )
            );
        }
    }

    private InMemoryStream FindStream(StreamName stream, bool failIfNotFound)
    {
        if (_storage.TryGetValue(stream, out var existing))
        {
            return existing;
        }

        return failIfNotFound 
            ? throw new StreamNotFound(stream) 
            : new InMemoryStream(stream);
    }
}

internal sealed record StoredEvent(StreamEvent Event, int Position);

internal sealed class InMemoryStream
{
    private readonly List<StoredEvent> _events;

    public InMemoryStream(StreamName name)
    {
        Name = name;
        Version = -1;
        _events = [];
    }

    public int Version { get; private set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public StreamName Name { get; }

    public void CheckVersion(ExpectedStreamVersion expectedVersion)
    {
        if (expectedVersion != ExpectedStreamVersion.Any && expectedVersion.Value != Version)
        {
            throw new WrongVersion(expectedVersion, Version);
        }
    }

    public void AppendEvents(ExpectedStreamVersion expectedVersion, IReadOnlyCollection<NewStreamEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        CheckVersion(expectedVersion);
        AppendEventsUnchecked(events);
    }

    public void AppendEventsUnchecked(IReadOnlyCollection<NewStreamEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var newEvent in events)
        {
            var version = ++Version;

            var streamEvent = new StreamEvent(
                newEvent.Id,
                newEvent.Payload,
                newEvent.Metadata,
                "application/json",
                version
            );

            _events.Add(new StoredEvent(streamEvent, version));
        }
    }

    public IEnumerable<StreamEvent> GetEvents(StreamReadPosition from, int count)
    {
        var selected = _events.SkipWhile(x => x.Position < from.Value);

        if (count > 0)
        {
            selected = selected.Take(count);
        }

        return selected.Select(x => x.Event with { Revision = x.Position });
    }

    public IEnumerable<StreamEvent> GetEventsBackwards(StreamReadPosition from, int count)
    {
        var position = (int)from.Value;

        while (count-- > 0)
        {
            yield return _events[position--].Event;
        }
    }

    public void Truncate(ExpectedStreamVersion version, StreamTruncatePosition position)
    {
        CheckVersion(version);
        _events.RemoveAll(x => x.Position <= position.Value);
    }
}

public sealed class WrongVersion
    : Exception
{
    public WrongVersion(ExpectedStreamVersion expected, int actual)
        : base($"Wrong stream version. Expected {expected.Value}, actual {actual}")
    {
    }
}