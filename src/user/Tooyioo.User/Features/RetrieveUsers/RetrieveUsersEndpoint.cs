using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Queries;
using Tooyioo.Common;
using Tooyioo.User.Features.RetrieveUsers.Contract;
// ReSharper disable UnusedType.Global

namespace Tooyioo.User.Features.RetrieveUsers;

public sealed class RetrieveUsersEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapQuery<
                RetrieveUsersRoute,
                RetrieveUsersQuery,
                RetrieveUsersQueryResult,
                RetrieveUsersResponse>(
                "/users",
                cfg => cfg
                    .WithSummary("Retrieves users")
                    .WithDescription("Retrieves paged users filtered by query parameters")
                    .WithTags("User"))
            .RequireRouteAuthorization<RetrieveUsersRoute, RetrieveUsersAuthorizer>()
            .WithQueryConventions<RetrieveUsersResponse, HttpProblemDetails>();
}
