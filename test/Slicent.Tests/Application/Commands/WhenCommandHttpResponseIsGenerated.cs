using System.Net;
using Microsoft.AspNetCore.Http;
using Slicent.Application;
using Slicent.Application.Commands;

namespace Slicent.Tests.Application.Commands;

public sealed class WhenCommandHttpResponseIsGenerated
{
    [Test]
    public async Task Then_ok_response_contains_payload()
    {
        var generator = new CommandHttpResponseGenerator<TestResponse>();
        var result = generator.Ok(new TestResponse { Id = "resource-1" });

        await Assert.That(result).IsAssignableTo<IValueHttpResult<TestResponse>>();
        var valueResult = (IValueHttpResult<TestResponse>)result;
        await Assert.That(valueResult.Value!.Id).IsEqualTo("resource-1");
    }

    [Test]
    public async Task Then_conflict_response_contains_problem_details()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/commands";
        var generator = new CommandHttpResponseGenerator<TestResponse>();

        var result = generator.Conflict(context, "already_exists", "Resource already exists.");

        await Assert.That(result).IsAssignableTo<IValueHttpResult<HttpProblemDetails>>();
        await Assert.That(result).IsAssignableTo<IStatusCodeHttpResult>();

        var valueResult = (IValueHttpResult<HttpProblemDetails>)result;
        var statusResult = (IStatusCodeHttpResult)result;

        await Assert.That(statusResult.StatusCode).IsEqualTo((int)HttpStatusCode.Conflict);
        await Assert.That(valueResult.Value!.Title).IsEqualTo("already_exists");
        await Assert.That(valueResult.Value.Detail).IsEqualTo("Resource already exists.");
        await Assert.That(valueResult.Value.Instance).IsEqualTo("/commands");
    }

    private sealed record TestResponse
    {
        public required string Id { get; init; }
    }
}
