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
                    .WithSummary("Completes profile")
                    .WithDescription("Completes profile with a unique alias and other contact details")
                    .WithTags("UserOnboarding"))
            .RequireRouteAuthorization<ChooseUserAliasRoute, ChooseUserAliasAuthorizer>()
            .WithCommandConventions<ChooseUserAliasResponse, HttpProblemDetails>();
}