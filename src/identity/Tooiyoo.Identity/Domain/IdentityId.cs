using Eventuous;

namespace Tooiyoo.Identity.Domain;

public sealed record IdentityId(string Value)
    : Id(Value)
{
    public static IdentityId New() => new(Guid.NewGuid().ToString());
    public static implicit operator IdentityId(string id) => new(id);
    public static implicit operator string(IdentityId id) => id.Value;
}