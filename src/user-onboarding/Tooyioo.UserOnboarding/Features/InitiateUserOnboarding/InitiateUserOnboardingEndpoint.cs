using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Contracts;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding;

public sealed class InitiateUserOnboardingEndpoint
    : IHttpEndpointModule
{
    // TODO Why do I need this? Why not private? is it used in policy?
    public const string InitiateUserOnboardingAuthPolicy = "InitiateUserOnboarding";

    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<InitiateUserOnboardingRequest, InitiateUserOnboardingCommand, InitiateUserOnboardingCommandResult,
                InitiateUserOnboardingResponse>(
                "user-onboarding/initiate",
                cfg
                    => cfg
                        .WithSummary("Initiates a user onboarding")
                        .WithDescription("Initiates a user onboarding with the information coming from external Id provider")
                        .WithTags("UserOnboarding"))
            .RequireAuthorization(InitiateUserOnboardingAuthPolicy)
            .WithCommandConventions<InitiateUserOnboardingResponse, HttpProblemDetails>();
}