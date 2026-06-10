using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Slicent.Application.Authorization;

public sealed class PayloadAuthorizeFilter<TRequest, TAuthorizer> 
    : IEndpointFilter
    where TAuthorizer : class, IPayloadAuthorizer<TRequest>
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var http = ctx.HttpContext;
        var request = FilterArg.Get<TRequest>(ctx);

        var authorizer = http.RequestServices.GetRequiredService<TAuthorizer>();
        var ok = await authorizer.Authorize(http.User, request, http, http.RequestAborted);

        return ok ? await next(ctx) : Results.Forbid();
    }
}