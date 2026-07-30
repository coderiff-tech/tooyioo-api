using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias.Contracts;

// ReSharper disable UnusedType.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserOnboardingAlias;

public sealed class ChooseUserOnboardingAliasEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                ChooseUserOnboardingAliasRoute,
                ChooseUserOnboardingAliasRequest,
                ChooseUserOnboardingAliasCommand,
                ChooseUserOnboardingAliasCommandResult,
                ChooseUserOnboardingAliasResponse>(
                "/user-onboarding/{id:guid}/choose-alias",
                cfg => cfg
                    .WithSummary("Chooses user onboarding alias")
                    .WithDescription("Chooses a globally unique alias as part of user onboarding")
                    .WithTags("UserOnboarding"))
            .RequireRouteAuthorization<ChooseUserOnboardingAliasRoute, ChooseUserOnboardingAliasAuthorizer>()
            .WithCommandConventions<ChooseUserOnboardingAliasResponse, HttpProblemDetails>();
}
