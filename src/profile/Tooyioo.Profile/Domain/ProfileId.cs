using Eventuous;

namespace Tooyioo.Profile.Domain;

public sealed record ProfileId(string Value)
    : Id(Value)
{
    public static ProfileId New() => new(Guid.NewGuid().ToString());
    public static implicit operator ProfileId(string id) => new(id);
    public static implicit operator string(ProfileId id) => id.Value;
}