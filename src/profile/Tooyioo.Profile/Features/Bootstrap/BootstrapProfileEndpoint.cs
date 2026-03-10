using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Commands;
using Tooiyoo.Common;
using Tooyioo.Profile.Features.Bootstrap.Contracts;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Bootstrap;

public sealed class BootstrapProfileEndpoint
    : IHttpEndpointModule
{
    public const string BootstrapAuthPolicy = "Bootstrap";

    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<BootstrapProfileRequest, BootstrapProfileCommand, BootstrapProfileCommandResult,
                BootstrapProfileResponse>(
                "profiles/bootstrap",
                cfg
                    => cfg
                        .WithSummary("Bootstraps a profile")
                        .WithDescription("Bootstraps a profile with the information coming from external Id provider")
                        .WithTags("Profile"))
            .RequireAuthorization(BootstrapAuthPolicy)
            .WithCommandConventions<BootstrapProfileResponse, HttpProblemDetails>();
}