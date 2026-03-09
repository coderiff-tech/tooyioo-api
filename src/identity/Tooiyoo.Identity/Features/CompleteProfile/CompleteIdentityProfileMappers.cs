using Microsoft.AspNetCore.Http;
using Slicent.Application;
using Tooiyoo.Identity.Domain;
using Tooiyoo.Identity.Features.CompleteProfile.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooiyoo.Identity.Features.CompleteProfile;

public static class CompleteIdentityProfileMappers
{
    public sealed class CompleteIdentityProfileCommandMapper
        : ICommandMapper<CompleteIdentityProfileRoute, CompleteIdentityProfileRequest, HttpContext, 
            CompleteIdentityProfileCommand>
    {
        public CompleteIdentityProfileCommand Map(
            CompleteIdentityProfileRoute urlParams,
            CompleteIdentityProfileRequest request, 
            HttpContext context)
            => new(
                new IdentityId(urlParams.IdentityId.ToString()),
                request.Alias.Trim(),
                request.PhoneNumber.Trim());
    }

    public sealed class ChangeIdentityEmailCommandHttpResponseMapper
        : ICommandHttpResponseMapper<CompleteIdentityProfileCommandResult, HttpContext, CompleteIdentityProfileResponse>
    {
        public IResult Map(
            CompleteIdentityProfileCommandResult commandResult,
            HttpContext context,
            CommandHttpResponseGenerator<CompleteIdentityProfileResponse> httpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => httpResponseGenerator.Ok(new CompleteIdentityProfileResponse { Id = ok.Id }),
                err => err.Match<IResult>(
                    aliasInUseError => httpResponseGenerator.BadRequest(
                        context,
                        "identity_alias_already_in_use",
                        $"The provided alias is already in use by Identity with Id {aliasInUseError.IdentityId}"),
                    _ => httpResponseGenerator.Conflict(
                        context,
                        "identity_concurrency_conflict",
                        "Could not complete the Identity profile because of a concurrency conflict. Please retry.")
                ));
    }
}
