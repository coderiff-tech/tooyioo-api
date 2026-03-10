using Microsoft.AspNetCore.Http;
using Slicent.Application.Commands;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Features.Complete.Contracts;

// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Complete;

public static class CompleteProfileMappers
{
    public sealed class CommandMapper
        : ICommandMapper<CompleteProfileRoute, CompleteProfileRequest, HttpContext, 
            CompleteIdentityProfileCommand>
    {
        public CompleteIdentityProfileCommand Map(
            CompleteProfileRoute urlParams,
            CompleteProfileRequest request, 
            HttpContext context)
            => new(
                new ProfileId(urlParams.ProfileId.ToString()),
                request.Alias.Trim(),
                request.PhoneNumber.Trim());
    }

    public sealed class CommandHttpResponseMapper
        : ICommandHttpResponseMapper<CompleteIdentityProfileCommandResult, HttpContext, CompleteProfileResponse>
    {
        public IResult Map(
            CompleteIdentityProfileCommandResult commandResult,
            HttpContext context,
            CommandHttpResponseGenerator<CompleteProfileResponse> commandHttpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => commandHttpResponseGenerator.Ok(new CompleteProfileResponse { Id = ok.Id }),
                err => err.Match<IResult>(
                    aliasInUseError => commandHttpResponseGenerator.BadRequest(
                        context,
                        "identity_alias_already_in_use",
                        $"The provided alias is already in use by profile with Id {aliasInUseError.ProfileId}"),
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "identity_concurrency_conflict",
                        "Could not complete the profile because of a concurrency conflict. Please retry.")
                ));
    }
}
