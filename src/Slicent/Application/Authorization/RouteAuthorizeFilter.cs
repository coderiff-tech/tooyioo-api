using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Slicent.Application.Authorization;

public sealed class RouteAuthorizeFilter<TRoute, TAuthorizer> 
    : IEndpointFilter
    where TAuthorizer : class, IRouteAuthorizer<TRoute>
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var http = ctx.HttpContext;
        var route = FilterArg.Get<TRoute>(ctx);

        var authorizer = http.RequestServices.GetRequiredService<TAuthorizer>();
        var ok = await authorizer.Authorize(http.User, route, http, http.RequestAborted);

        return ok ? await next(ctx) : Results.Forbid();
    }
}