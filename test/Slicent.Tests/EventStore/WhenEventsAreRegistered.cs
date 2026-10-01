using System.Reflection;
using Eventuous;
using Eventuous.KurrentDB;
using Microsoft.Extensions.DependencyInjection;
using EventA = Slicent.TestEventsA.SampleEvent;
using EventB = Slicent.TestEventsB.SampleEvent;

namespace Slicent.Tests.EventStore;

public sealed class WhenEventsAreRegistered
{
    [Test]
    public async Task Then_hosts_have_isolated_type_maps()
    {
        var servicesA = new ServiceCollection();
        servicesA.AddSlicent(typeof(EventA).Assembly);
        var servicesB = new ServiceCollection();
        servicesB.AddSlicent(typeof(EventB).Assembly);

        await using var providerA = servicesA.BuildServiceProvider();
        await using var providerB = servicesB.BuildServiceProvider();
        var serializerA = providerA.GetRequiredService<IEventSerializer>();
        var serializerB = providerB.GetRequiredService<IEventSerializer>();

        var storedA = serializerA.SerializeEvent(new EventA("first"));
        var storedB = serializerB.SerializeEvent(new EventB(42));
        var readA = serializerA.DeserializeEvent(storedA.Payload, storedA.EventType, storedA.ContentType);
        var readB = serializerB.DeserializeEvent(storedB.Payload, storedB.EventType, storedB.ContentType);

        await Assert.That(storedA.EventType).IsEqualTo("Test.SharedName");
        await Assert.That(storedB.EventType).IsEqualTo("Test.SharedName");
        await Assert.That(((DeserializationResult.SuccessfullyDeserialized)readA).Payload).IsEqualTo(new EventA("first"));
        await Assert.That(((DeserializationResult.SuccessfullyDeserialized)readB).Payload).IsEqualTo(new EventB(42));
        await Assert.That(providerA.GetRequiredService<ITypeMapper>()).IsNotEqualTo(providerB.GetRequiredService<ITypeMapper>());
    }

    [Test]
    public async Task Then_kurrentdb_store_resolves_without_a_running_server()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSlicent(typeof(EventA).Assembly);
        // Only DI construction is checked; no KurrentDB request or test container is started.
        services.AddSlicentKurrentDb("kurrentdb://unused.invalid:2113?tls=false");

        await using var provider = services.BuildServiceProvider();

        await Assert.That(provider.GetRequiredService<IEventStore>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<KurrentDBEventStore>()).IsNotNull();
        await Assert.That(provider.GetRequiredService<IEventSerializer>()).IsNotNull();
    }

    [Test]
    public async Task Then_duplicate_names_fail_registration()
    {
        var exception = RegisterAndCapture(typeof(EventA).Assembly, typeof(EventB).Assembly);

        await Assert.That(exception).IsTypeOf<InvalidOperationException>();
        await Assert.That(exception!.Message.Contains("Test.SharedName", StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task Then_unknown_type_and_wrong_content_type_return_failures()
    {
        var services = new ServiceCollection();
        services.AddSlicent(typeof(EventA).Assembly);
        await using var provider = services.BuildServiceProvider();
        var serializer = provider.GetRequiredService<IEventSerializer>();
        var bytes = "{}"u8;

        var unknown = serializer.DeserializeEvent(bytes, "Test.Unknown", "application/json");
        var wrongContentType = serializer.DeserializeEvent(bytes, "Test.SharedName", "text/plain");

        await Assert.That(((DeserializationResult.FailedToDeserialize)unknown).Error).IsEqualTo(DeserializationError.UnknownType);
        await Assert.That(((DeserializationResult.FailedToDeserialize)wrongContentType).Error).IsEqualTo(DeserializationError.ContentTypeMismatch);
    }

    private static Exception? RegisterAndCapture(params Assembly[] assemblies)
    {
        try
        {
            new ServiceCollection().AddSlicent(assemblies);
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}
