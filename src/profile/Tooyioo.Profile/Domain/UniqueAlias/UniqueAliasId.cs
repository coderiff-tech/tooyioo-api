// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

using Eventuous;

namespace Tooyioo.Profile.Domain.UniqueAlias;

public sealed record UniqueAliasId(string Value)
    : Id(Value)
{
    public static implicit operator UniqueAliasId(string id) => new(id);
    public static implicit operator string(UniqueAliasId id) => id.Value;
}