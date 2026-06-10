using Eventuous.Projections.MongoDB.Tools;
using MongoDB.Bson.Serialization.Attributes;

namespace Slicent.Application.Queries;

public abstract record Document(string Id)
    : ProjectedDocument(Id)
{
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; init; } = DateTime.MinValue;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastModifiedAt { get; init; } = DateTime.MinValue;
}