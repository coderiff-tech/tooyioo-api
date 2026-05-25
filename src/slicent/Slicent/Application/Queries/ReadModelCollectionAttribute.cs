namespace Slicent.Application.Queries;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ReadModelCollectionAttribute(string collectionName)
    : Attribute
{
    public string CollectionName { get; } = collectionName;
}
