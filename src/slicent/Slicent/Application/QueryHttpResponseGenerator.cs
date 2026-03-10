using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Queries;

public sealed class QueryHttpResponseGenerator<THttpResponse>
    where THttpResponse : class
{
    public IResult Ok(THttpResponse response) => TypedResults.Ok(response);

    public IResult BadRequest(HttpContext ctx, string title, string? detail = null)
        => TypedResults.BadRequest(HttpProblemDetails.BadRequest(ctx, title, detail));

    public IResult NotFound(HttpContext ctx, string title, string? detail = null)
        => TypedResults.NotFound(HttpProblemDetails.NotFound(ctx, title, detail));

    public IResult Forbidden(HttpContext ctx, string title = "forbidden", string? detail = null)
        => TypedResults.Problem(HttpProblemDetails.FromForbidden(ctx, title, detail));
}