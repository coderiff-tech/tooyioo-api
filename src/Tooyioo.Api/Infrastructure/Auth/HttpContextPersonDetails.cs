using Tooyioo.Profile.Features.Bootstrap.Support;

namespace Tooyioo.Api.Infrastructure.Auth;

internal static class HttpContextPersonalDetails
{
    private static readonly object Key = new();

    public static void Set(HttpContext ctx, PersonalDetails details)
        => ctx.Items[Key] = details;

    public static PersonalDetails? TryGet(HttpContext ctx)
        => ctx.Items.TryGetValue(Key, out var value) ? value as PersonalDetails : null;
}
