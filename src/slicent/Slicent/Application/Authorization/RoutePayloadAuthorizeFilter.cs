using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Slicent.Application.Authorization;

public sealed class RoutePayloadAuthorizeFilter<TRoute, TRequest, TAuthorizer> 
    : IEndpointFilter
    where TAuthorizer : class, IRoutePayloadAuthorizer<TRoute, TRequest>
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var http = ctx.HttpContext;
        var route = FilterArg.Get<TRoute>(ctx);
        var request = FilterArg.Get<TRequest>(ctx);

        var authorizer = http.RequestServices.GetRequiredService<TAuthorizer>();
        var ok = await authorizer.Authorize(http.User, route, request, http, http.RequestAborted);

        return ok ? await next(ctx) : Results.Forbid();
    }
}