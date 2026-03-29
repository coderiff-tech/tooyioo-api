using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooiyoo.Common;
using Tooyioo.Profile.Features.Complete.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Complete;

public sealed class CompleteProfileEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                CompleteProfileRoute,
                CompleteProfileRequest,
                CompleteIdentityProfileCommand,
                CompleteProfileCommandResult,
                CompleteProfileResponse>(
                "/profiles/{id:guid}/complete",
                cfg => cfg
                    .WithSummary("Completes profile")
                    .WithDescription("Completes profile with a unique alias and other contact details")
                    .WithTags("Profile"))
            .RequireRouteAuthorization<CompleteProfileRoute, CompleteProfileAuthorizer>()
            .WithCommandConventions<CompleteProfileResponse, HttpProblemDetails>();
}