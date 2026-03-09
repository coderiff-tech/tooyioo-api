using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Authorization;

internal static class FilterArg
{
    public static T Get<T>(EndpointFilterInvocationContext ctx)
    {
        foreach (var arg in ctx.Arguments)
        {
            if (arg is T t)
            {
                return t;
            }
        }

        throw new InvalidOperationException(
            $"Expected handler argument of type {typeof(T).Name}. " +
            $"Ensure the endpoint handler has a parameter of that type.");
    }
}