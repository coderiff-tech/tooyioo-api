using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.CompleteUserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding.Features.CompleteUserOnboarding;

public sealed class CompleteUserOnboardingEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                CompleteUserOnboardingRoute,
                CompleteUserOnboardingRequest,
                CompleteUserOnboardingCommand,
                CompleteUserOnboardingCommandResult,
                CompleteUserOnboardingResponse>(
                "/user-onboarding/{id:guid}/complete",
                cfg => cfg
                    .WithSummary("Completes user onboarding")
                    .WithDescription("Completes user onboarding process")
                    .WithTags("UserOnboarding"))
            .RequireRouteAuthorization<CompleteUserOnboardingRoute, CompleteUserOnboardingAuthorizer>()
            .WithCommandConventions<CompleteUserOnboardingResponse, HttpProblemDetails>();
}