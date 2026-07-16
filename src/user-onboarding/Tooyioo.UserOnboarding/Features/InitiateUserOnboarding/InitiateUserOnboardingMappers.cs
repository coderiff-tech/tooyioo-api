using Microsoft.AspNetCore.Http;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Contracts;
using Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding;

public static class InitiateUserOnboardingMappers
{
    public sealed class CommandMapper
        : ICommandMapper<InitiateUserOnboardingRequest, HttpContext, InitiateUserOnboardingCommand>
    {
        private readonly IPersonalDetailsRetriever _personalDetailsRetriever;

        public CommandMapper(IPersonalDetailsRetriever personalDetailsRetriever)
        {
            _personalDetailsRetriever = personalDetailsRetriever;
        }
        
        public InitiateUserOnboardingCommand Map(InitiateUserOnboardingRequest request, HttpContext context)
        {
            var personalDetails = _personalDetailsRetriever.GetPersonalDetailsFromContext(context);

            var subOption = context.User.GetSubClaim();
            var externalId = subOption.ValueOr(string.Empty);

            return new InitiateUserOnboardingCommand(
                UserOnboardingId.New(),
                personalDetails.Name.Trim(),
                personalDetails.LastName.Trim(),
                personalDetails.Email.Trim(),
                personalDetails.IsEmailVerified,
                externalId.Trim(),
                request.ExternalIdProvider);
        }
    }

    public sealed class CommandHttpResponseMapper
        : ICommandHttpResponseMapper<InitiateUserOnboardingCommandResult, HttpContext, InitiateUserOnboardingResponse>
    {
        public IResult Map(
            InitiateUserOnboardingCommandResult commandResult, 
            HttpContext context,
            CommandHttpResponseGenerator<InitiateUserOnboardingResponse> commandHttpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => commandHttpResponseGenerator.Ok(new InitiateUserOnboardingResponse
                {
                    UserOnboardingId = ok.UserOnboardingId, 
                    ExternalId = ok.ExternalId, 
                    ExternalIdProvider = ok.ExternalIdProvider
                }),
                err => err.Match<IResult>(
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "initiate_user_onboarding_concurrency_conflict",
                        "Could not initiate user onboarding because of a concurrency conflict, please retry"),
                    _ => commandHttpResponseGenerator.Conflict(
                        context,
                        "initiate_user_onboarding_unexpected_state_conflict",
                        "Could not initiate user onboarding because of an unexpected state conflict, please retry")
                ));
    }
}
