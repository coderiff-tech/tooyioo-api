using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Slicent.Application;
using Slicent.Application.Authorization;
using Tooiyoo.Common;
using Tooiyoo.Identity.Features.CompleteProfile.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooiyoo.Identity.Features.CompleteProfile;

public sealed class CompleteIdentityProfileEndpoint
    : IHttpEndpointModule
{
    // public void Map(IEndpointRouteBuilder app)
    //     => app
    //         .Command<CompleteIdentityProfileRoute, CompleteIdentityProfileRequest, CompleteIdentityProfileCommand,
    //             CompleteIdentityProfileCommandResult, CompleteIdentityProfileResponse, CompleteIdentityProfileAuthorizer>(
    //             "/identities/{id:guid}/completeProfile",
    //             cfg 
    //                 => cfg
    //                     .WithSummary("Completes identity's profile")
    //                     .WithDescription("""
    //                                      Completes identity's profile with a unique alias and other contact details'
    //                                      """
    //                     ).WithTags("Identity"));


    public void Map(IEndpointRouteBuilder app)
        => app
            .MapCommand<
                CompleteIdentityProfileRoute,
                CompleteIdentityProfileRequest,
                CompleteIdentityProfileCommand,
                CompleteIdentityProfileCommandResult,
                CompleteIdentityProfileResponse>(
                "/identities/{id:guid}/completeProfile",
                cfg => cfg
                    .WithSummary("Completes identity's profile")
                    .WithDescription("Completes identity's profile with a unique alias and other contact details")
                    .WithTags("Identity"))
            .RequireRouteAuthorization<CompleteIdentityProfileRoute, CompleteIdentityProfileAuthorizer>()
            .WithCommandConventions<CompleteIdentityProfileResponse, HttpProblemDetails>();
}