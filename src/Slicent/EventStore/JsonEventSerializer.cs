using System.Text.Json;
using Eventuous;

namespace Slicent.EventStore;

internal sealed class JsonEventSerializer(ITypeMapper typeMapper)
    : IEventSerializer
{
    private const string ContentType = "application/json";

    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public SerializationResult SerializeEvent(object evt)
    {
        var eventType = typeMapper.GetTypeName(evt);
        var payload = JsonSerializer.SerializeToUtf8Bytes(evt, evt.GetType(), _options);
        return new SerializationResult(eventType, ContentType, payload);
    }

    public DeserializationResult DeserializeEvent(ReadOnlySpan<byte> data, string eventType, string contentType)
    {
        if (!typeMapper.TryGetType(eventType, out var type))
        {
            return new DeserializationResult.FailedToDeserialize(DeserializationError.UnknownType);
        }

        if (contentType != ContentType)
        {
            return new DeserializationResult.FailedToDeserialize(DeserializationError.ContentTypeMismatch);
        }

        var payload = JsonSerializer.Deserialize(data, type, _options);
        return payload is null
            ? new DeserializationResult.FailedToDeserialize(DeserializationError.PayloadEmpty)
            : new DeserializationResult.SuccessfullyDeserialized(payload);
    }
}
