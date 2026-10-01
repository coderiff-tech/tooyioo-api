using System.Reflection;
using Eventuous;

namespace Slicent.EventStore;

internal static class EventTypeMapperFactory
{
    public static TypeMapper Create(IReadOnlyCollection<Assembly> assemblies)
    {
        var mapper = new TypeMapper();
        var names = new Dictionary<string, Type>(StringComparer.Ordinal);

        foreach (var type in assemblies.SelectMany(assembly => assembly.DefinedTypes))
        {
            var attribute = type.GetCustomAttribute<DomainEventTypeAttribute>();
            if (attribute is null)
            {
                continue;
            }

            if (!type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
            {
                throw new InvalidOperationException($"Event type {type.FullName} must be a concrete, closed class.");
            }

            if (string.IsNullOrWhiteSpace(attribute.Name))
            {
                throw new InvalidOperationException($"Event type {type.FullName} must have a non-empty stored name.");
            }

            if (names.TryGetValue(attribute.Name, out var existing))
            {
                throw new InvalidOperationException(
                    $"Stored event name '{attribute.Name}' is used by both {existing.FullName} and {type.FullName}.");
            }

            names.Add(attribute.Name, type);
            mapper.AddType(type, attribute.Name);
        }

        return mapper;
    }
}
