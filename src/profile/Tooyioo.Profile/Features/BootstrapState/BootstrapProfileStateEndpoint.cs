using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.Profile.Features.Bootstrap.Contracts;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.BootstrapState;

public sealed class BootstrapProfileStateEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<BootstrapProfileRequest, BootstrapProfileStateCommand, BootstrapProfileStateCommandResult,
                BootstrapProfileResponse>(
                "profiles/bootstrap-state",
                cfg
                    => cfg
                        .WithSummary("Bootstraps a profile without an aggregate")
                        .WithDescription(
                            "Bootstraps a profile by loading Eventuous state directly instead of loading aggregates")
                        .WithTags("Profile"))
            .RequireAuthorization(Bootstrap.BootstrapProfileEndpoint.BootstrapAuthPolicy)
            .WithCommandConventions<BootstrapProfileResponse, HttpProblemDetails>();
}
