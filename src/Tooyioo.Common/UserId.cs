namespace Tooyioo.Common;

public sealed record UserId(string Id)
{
    public static UserId New() => new(Guid.NewGuid().ToString());
    public static implicit operator UserId(string id) => new(id);
    public static implicit operator string(UserId id) => id.Id;
}
