using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Queries;
using Tooyioo.Common;
using Tooyioo.Profile.Features.Retrieve.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Retrieve;

public sealed class RetrieveProfileEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapQuery<
                RetrieveProfileRoute,
                RetrieveProfileQuery,
                RetrieveProfileQueryResult,
                RetrieveProfileResponse>(
                "/profiles/{id:guid}",
                cfg => cfg
                    .WithSummary("Retrieves a profile")
                    .WithDescription("Retrieves a profile by its Id")
                    .WithTags("Profile"))
            .RequireRouteAuthorization<RetrieveProfileRoute, RetrieveProfileAuthorizer>()
            .WithQueryConventions<RetrieveProfileResponse, HttpProblemDetails>();
}