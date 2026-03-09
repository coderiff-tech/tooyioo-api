
// ReSharper disable ConvertToExtensionBlock

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Authorization;

public static class SliceAuthorizationExtensions
{
    public static RouteHandlerBuilder RequirePayloadAuthorization<TRequest, TAuthorizer>(
        this RouteHandlerBuilder builder)
        where TAuthorizer : class, IPayloadAuthorizer<TRequest>
        => builder.RequireAuthorization().AddEndpointFilter<PayloadAuthorizeFilter<TRequest, TAuthorizer>>();

    public static RouteHandlerBuilder RequireRouteAuthorization<TRoute, TAuthorizer>(this RouteHandlerBuilder builder)
        where TRoute : notnull
        where TAuthorizer : class, IRouteAuthorizer<TRoute>
        => builder.RequireAuthorization().AddEndpointFilter<RouteAuthorizeFilter<TRoute, TAuthorizer>>();

    public static RouteHandlerBuilder RequireRoutePayloadAuthorization<TRoute, TRequest, TAuthorizer>(this RouteHandlerBuilder builder)
        where TRoute : notnull
        where TRequest : class
        where TAuthorizer : class, IRoutePayloadAuthorizer<TRoute, TRequest>
        => builder.RequireAuthorization().AddEndpointFilter<RoutePayloadAuthorizeFilter<TRoute, TRequest, TAuthorizer>>();
}