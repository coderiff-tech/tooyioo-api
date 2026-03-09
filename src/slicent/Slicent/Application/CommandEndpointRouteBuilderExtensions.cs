using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application.Commands;
// ReSharper disable ConvertToExtensionBlock

namespace Slicent.Application;

public static class CommandEndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapCommand<TRequest, TCommand, TResult, TResponse>(
        this IEndpointRouteBuilder app,
        string pattern,
        Action<RouteHandlerBuilder>? configure = null)
        where TRequest : class
        where TCommand : ICommand<TResult>
        where TResponse : class
    {
        var builder = app.MapPost(pattern, async (
            TRequest request,
            HttpContext ctx,
            ICommandMapper<TRequest, HttpContext, TCommand> commandMapper,
            ICommandDispatcher dispatcher,
            ICommandHttpResponseMapper<TResult, HttpContext, TResponse> responseMapper,
            CommandHttpResponseGenerator<TResponse> gen) =>
        {
            var command = commandMapper.Map(request, ctx);
            var result = await dispatcher.Send(command, ctx.RequestAborted);
            return responseMapper.Map(result, ctx, gen);
        });

        configure?.Invoke(builder);
        return builder;
    }

    public static RouteHandlerBuilder MapCommand<TRoute, TRequest, TCommand, TResult, TResponse>(
        this IEndpointRouteBuilder app,
        string pattern,
        Action<RouteHandlerBuilder>? configure = null)
        where TRoute : notnull
        where TRequest : class
        where TCommand : ICommand<TResult>
        where TResponse : class
    {
        var builder = app.MapPost(pattern, async (
            [AsParameters] TRoute route,
            TRequest request,
            HttpContext ctx,
            ICommandMapper<TRoute, TRequest, HttpContext, TCommand> commandMapper,
            ICommandDispatcher dispatcher,
            ICommandHttpResponseMapper<TResult, HttpContext, TResponse> responseMapper,
            CommandHttpResponseGenerator<TResponse> gen) =>
        {
            var command = commandMapper.Map(route, request, ctx);
            var result = await dispatcher.Send(command, ctx.RequestAborted);
            return responseMapper.Map(result, ctx, gen);
        });

        configure?.Invoke(builder);
        return builder;
    }
}
