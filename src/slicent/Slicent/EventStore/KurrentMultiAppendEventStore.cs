using System.Runtime.CompilerServices;
using Eventuous;
using Eventuous.KurrentDB;
using KurrentDB.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Slicent.EventStore;

internal sealed class KurrentMultiAppendEventStore 
    : IEventStore, IMultiAppendEventWriter 
{
    private readonly KurrentDBClient _client;
    private readonly IEventSerializer _serializer;
    private readonly IMetadataSerializer _metaSerializer;
    private readonly ILogger _logger;
    private readonly KurrentDBEventStore _inner;

    public KurrentMultiAppendEventStore(
        KurrentDBClient client,
        KurrentDBEventStore inner,
        IEventSerializer? serializer = null,
        IMetadataSerializer? metaSerializer = null,
        ILogger<KurrentMultiAppendEventStore>? logger = null) 
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
        _serializer = serializer ?? DefaultEventSerializer.Instance;
        _metaSerializer = metaSerializer ?? DefaultMetadataSerializer.Instance;
        _logger = logger ?? NullLogger<KurrentMultiAppendEventStore>.Instance;
        _inner = inner;
    }

    public KurrentMultiAppendEventStore(
        KurrentDBClientSettings clientSettings,
        KurrentDBEventStore inner,
        IEventSerializer? serializer = null,
        IMetadataSerializer? metaSerializer = null,
        ILogger<KurrentMultiAppendEventStore>? logger = null)
        : this(new KurrentDBClient(clientSettings), inner, serializer, metaSerializer, logger)
    {
        ArgumentNullException.ThrowIfNull(clientSettings);
    }
    
    public Task<bool> StreamExists(StreamName stream, CancellationToken cancellationToken = default)
        => _inner.StreamExists(stream, cancellationToken);

    public Task<AppendEventsResult> AppendEvents(
        StreamName stream,
        ExpectedStreamVersion expectedVersion,
        IReadOnlyCollection<NewStreamEvent> events,
        CancellationToken cancellationToken = default)
        => _inner.AppendEvents(stream, expectedVersion, events, cancellationToken);
    
    public IAsyncEnumerable<StreamEvent> ReadEvents(StreamName stream, StreamReadPosition start, int count, CancellationToken cancellationToken)
        => _inner.ReadEvents(stream, start, count, cancellationToken);

    public IAsyncEnumerable<StreamEvent> ReadEventsBackwards(StreamName stream, StreamReadPosition start, int count, CancellationToken cancellationToken)
        => _inner.ReadEventsBackwards(stream, start, count, cancellationToken);
    
    public Task TruncateStream(StreamName stream, StreamTruncatePosition truncatePosition, ExpectedStreamVersion expectedVersion, CancellationToken cancellationToken)
        => _inner.TruncateStream(stream, truncatePosition, expectedVersion, cancellationToken);

    public Task DeleteStream(StreamName stream, ExpectedStreamVersion expectedVersion, CancellationToken cancellationToken = default)
        => _inner.DeleteStream(stream, expectedVersion, cancellationToken);

    public async Task<MultiAppendEventsResult> AppendEvents(
        IReadOnlyCollection<AppendEventsRequest> appendEventsRequests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(appendEventsRequests);

        var reqs = appendEventsRequests
            .Where(r => r.Changes is { Count: > 0 })
            .ToArray();

        if (reqs.Length == 0)
        {
            return new MultiAppendEventsResult(0, []);
        }

        var duplicate = reqs
            .GroupBy(r => r.StreamName)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate is not null)
        {
            throw new ArgumentException(
                $"Same stream appears multiple times in one append: {duplicate.Key}",
                nameof(appendEventsRequests));
        }

        try
        {
            var kurrentRequests = reqs.Select(r =>
            {
                var streamEvents = r.Changes
                    .Select(payload =>
                    {
                        var se = new NewStreamEvent(Guid.NewGuid(), payload, new Metadata());
                        return r.AmendEvent?.Invoke(se) ?? se;
                    })
                    .ToArray();

                var eventData = streamEvents.Select(ToEventData).ToArray();

                return new AppendStreamRequest(
                    Stream: r.StreamName,
                    ExpectedState: ToStreamState(r.ExpectedVersion),
                    Messages: eventData);
            }).ToArray();

            var result = await _client
                .MultiStreamAppendAsync(kurrentRequests, cancellationToken)
                .ConfigureAwait(false);

            var globalPosition = checked((ulong)result.Position);

            var perStream = (result.Responses ?? [])
                .Select(r => new StreamAppendResult(new StreamName(r.Stream), r.StreamRevision))
                .ToArray();

            return new MultiAppendEventsResult(globalPosition, perStream);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to append events to multiple streams");
            throw;
        }

        EventData ToEventData(NewStreamEvent streamEvent)
        {
            var (eventType, contentType, payload) = _serializer.SerializeEvent(streamEvent.Payload!);

            return new EventData(
                Uuid.FromGuid(streamEvent.Id),
                eventType,
                payload,
                _metaSerializer.Serialize(streamEvent.Metadata),
                contentType);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StreamState ToStreamState(ExpectedStreamVersion version)
        => version == ExpectedStreamVersion.NoStream
            ? StreamState.NoStream
            : version == ExpectedStreamVersion.Any
                ? StreamState.Any
                : StreamState.StreamRevision((ulong)version.Value);
}