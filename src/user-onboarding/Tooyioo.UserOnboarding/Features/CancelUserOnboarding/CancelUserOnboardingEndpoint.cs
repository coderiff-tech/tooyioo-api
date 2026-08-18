using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.CancelUserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding.Features.CancelUserOnboarding;

public sealed class CancelUserOnboardingEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                CancelUserOnboardingRoute,
                CancelUserOnboardingRequest,
                CancelUserOnboardingCommand,
                CancelUserOnboardingCommandResult,
                CancelUserOnboardingResponse>(
                "/user-onboarding/{id:guid}/cancel",
                cfg => cfg
                    .WithName("Cancel User Onboarding")
                    .WithSummary("Cancels user onboarding")
                    .WithDescription("Cancels an in-progress user onboarding process")
                    .WithTags("UserOnboarding"))
            .RequireRouteAuthorization<CancelUserOnboardingRoute, CancelUserOnboardingAuthorizer>()
            .WithCommandConventions<CancelUserOnboardingResponse, HttpProblemDetails>();
}
