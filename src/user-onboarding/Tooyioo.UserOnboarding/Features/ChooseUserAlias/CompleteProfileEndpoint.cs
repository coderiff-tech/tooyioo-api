using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;

// ReSharper disable UnusedType.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias;

public sealed class CompleteProfileEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                ChooseUserAliasRoute,
                ChooseUserAliasRequest,
                ChooseUserAliasCommand,
                ChooseUserAliasCommandResult,
                ChooseUserAliasResponse>(
                "/user-onboarding/{id:guid}/choose-alias",
                cfg => cfg
                    .WithSummary("Chooses user alias")
                    .WithDescription("Chooses user globally unique alias as part of user onboarding")
                    .WithTags("UserOnboarding"))
            .RequireRouteAuthorization<ChooseUserAliasRoute, ChooseUserAliasAuthorizer>()
            .WithCommandConventions<ChooseUserAliasResponse, HttpProblemDetails>();
}