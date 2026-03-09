using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Tooiyoo.Common;
using Tooiyoo.Identity.Features.Bootstrap.Contracts;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedType.Global

namespace Tooiyoo.Identity.Features.Bootstrap;

public sealed class BootstrapIdentityEndpoint
    : IHttpEndpointModule
{
    public const string BootstrapAuthPolicy = "Bootstrap";

    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<BootstrapIdentityRequest, BootstrapIdentityCommand, BootstrapIdentityCommandResult,
                BootstrapIdentityResponse>(
                "identities/bootstrap",
                cfg
                    => cfg
                        .WithSummary("Bootstraps an identity")
                        .WithDescription("""
                                         Bootstraps an identity with the external provider information, creating a mapping
                                         between the external Id and the internal Id.
                                         """
                        ).WithTags("Identity"))
            .RequireAuthorization(BootstrapAuthPolicy)
            .WithCommandConventions<BootstrapIdentityResponse, HttpProblemDetails>();
}