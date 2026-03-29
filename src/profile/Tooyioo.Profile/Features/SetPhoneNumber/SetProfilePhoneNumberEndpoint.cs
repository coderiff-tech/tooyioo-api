using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Slicent.Application.Commands;
using Tooiyoo.Common;
using Tooyioo.Profile.Features.SetPhoneNumber.Contracts;

// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.SetPhoneNumber;

public sealed class SetProfilePhoneNumberEndpoint
    : IHttpEndpointModule
{
    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                SetProfilePhoneNumberRoute,
                SetProfilePhoneNumberRequest,
                SetProfilePhoneNumberCommand,
                SetProfilePhoneNumberCommandResult,
                SetProfilePhoneNumberResponse>(
                "/profiles/{id:guid}/setPhoneNumber",
                cfg => cfg
                    .WithSummary("Sets profile phone number")
                    .WithDescription("Sets profile phone number")
                    .WithTags("Profile"))
            .RequireRouteAuthorization<SetProfilePhoneNumberRoute, SetProfilePhoneNumberAuthorizer>()
            .WithCommandConventions<SetProfilePhoneNumberResponse, HttpProblemDetails>();
}