using System.ComponentModel.DataAnnotations;

namespace Tooyioo.Api.Infrastructure.Slicent;

public sealed class SlicentOptions
{
    public const string SectionName = "Slicent";

    [EnumDataType(typeof(EventStoreProvider))]
    public EventStoreProvider EventStoreProvider { get; init; } = EventStoreProvider.KurrentDb;
}
