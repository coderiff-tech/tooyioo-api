// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
using Eventuous;

namespace Tooiyoo.Identity.Domain.UniqueExternalIdentity;

public sealed record UniqueExternalIdentityId(string Value)
    : Id(Value)
{
    public static implicit operator UniqueExternalIdentityId(string id) => new(id);
    public static implicit operator string(UniqueExternalIdentityId id) => id.Value;
}