using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Tooyioo.Common;

public static class CommandOpenApiConventions
{
    private const string Json = "application/json";
    private const string ProblemJson = "application/problem+json";

    public static RouteHandlerBuilder WithCommandConventions<TResponse, TProblemDetails>(
        this RouteHandlerBuilder builder,
        bool includeAuth = true)
        where TResponse : class
    {
        builder
            .Produces<TResponse>(StatusCodes.Status200OK, Json)
            .Produces<TProblemDetails>(StatusCodes.Status400BadRequest, ProblemJson)
            .Produces<TProblemDetails>(StatusCodes.Status409Conflict, ProblemJson)
            .Produces<TProblemDetails>(StatusCodes.Status422UnprocessableEntity, ProblemJson)
            .Produces<TProblemDetails>(StatusCodes.Status404NotFound, ProblemJson);

        if (includeAuth)
        {
            builder
                .Produces<TProblemDetails>(StatusCodes.Status401Unauthorized, ProblemJson)
                .Produces<TProblemDetails>(StatusCodes.Status403Forbidden, ProblemJson);
        }
        
        return builder;
    }
}
