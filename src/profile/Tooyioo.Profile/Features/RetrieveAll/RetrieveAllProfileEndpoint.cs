using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Queries;
using Tooyioo.Common;
using Tooyioo.Profile.Features.RetrieveAll.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.RetrieveAll;

public sealed class RetrieveAllProfileEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapQuery<
                RetrieveAllProfileRoute,
                RetrieveAllProfileQuery,
                RetrieveAllProfileQueryResult,
                RetrieveAllProfileResponse>(
                "/profiles",
                cfg => cfg
                    .WithSummary("Retrieves profiles")
                    .WithDescription("Retrieves paged profiles filtered by query parameters")
                    .WithTags("Profile"))
            .RequireRouteAuthorization<RetrieveAllProfileRoute, RetrieveAllProfileAuthorizer>()
            .WithQueryConventions<RetrieveAllProfileResponse, HttpProblemDetails>();
}
