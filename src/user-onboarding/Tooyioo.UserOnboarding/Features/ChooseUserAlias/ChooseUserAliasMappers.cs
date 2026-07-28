using Microsoft.AspNetCore.Http;
using Slicent.Application.Commands;
using Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;
// ReSharper disable UnusedType.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias;

public static class ChooseUserAliasMappers
{
    public sealed class CommandMapper
        : ICommandMapper<ChooseUserAliasRoute, ChooseUserAliasRequest, HttpContext, 
            ChooseUserAliasCommand>
    {
        public ChooseUserAliasCommand Map(
            ChooseUserAliasRoute urlParams,
            ChooseUserAliasRequest request, 
            HttpContext context)
            => new(
                new UserOnboardingId(urlParams.UserOnboardingId.ToString()),
                request.Alias.Trim());
    }

    public sealed class CommandHttpResponseMapper
        : ICommandHttpResponseMapper<ChooseUserAliasCommandResult, HttpContext, ChooseUserAliasResponse>
    {
        public IResult Map(
            ChooseUserAliasCommandResult commandResult,
            HttpContext context,
            CommandHttpResponseGenerator<ChooseUserAliasResponse> commandHttpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => commandHttpResponseGenerator.Ok(new ChooseUserAliasResponse { UserOnboardingId = ok.UserOnboardingId }),
                err => err.Match<IResult>(
                    userAliasInUseError => commandHttpResponseGenerator.BadRequest(
                        context,
                        "choose_user_alias_already_in_use",
                        $"The provided user alias is already in use by user with Id {userAliasInUseError.UserOnboardingId}"),
                    userAliasAlreadyChosenError => commandHttpResponseGenerator.BadRequest(
                        context,
                        "choose_user_different_alias_already_chosen",
                        $"A different alias {userAliasAlreadyChosenError.ExistingAlias} had already been chosen for this user"),
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "choose_user_alias_concurrency_conflict",
                        "Could not choose user alias because of a concurrency conflict, please retry"),
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "choose_user_alias_unexpected_state_conflict",
                        "Could not choose user alias because of an unexpected state conflict, please retry"),
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "choose_user_alias_canceled_conflict",
                        "Could not choose user alias because user onboarding has been canceled")
                ));
    }
}
