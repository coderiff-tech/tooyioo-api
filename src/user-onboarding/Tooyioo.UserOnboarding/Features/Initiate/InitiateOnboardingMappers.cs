using Microsoft.AspNetCore.Http;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.UserOnboarding.Features.Initiate.Contracts;
using Tooyioo.UserOnboarding.Features.Initiate.Support;
// ReSharper disable ConvertToPrimaryConstructor

namespace Tooyioo.UserOnboarding.Features.Initiate;

public static class InitiateOnboardingMappers
{
    public sealed class CommandMapper
        : ICommandMapper<InitiateUserOnboardingRequest, HttpContext, InitiateOnboardingCommand>
    {
        private readonly IPersonalDetailsRetriever _personalDetailsRetriever;

        public CommandMapper(IPersonalDetailsRetriever personalDetailsRetriever)
        {
            _personalDetailsRetriever = personalDetailsRetriever;
        }
        
        public InitiateOnboardingCommand Map(InitiateUserOnboardingRequest request, HttpContext context)
        {
            var personalDetails = _personalDetailsRetriever.GetPersonalDetailsFromContext(context);

            var subOption = context.User.GetSubClaim();
            var externalId = subOption.ValueOr(string.Empty);

            return new InitiateOnboardingCommand(
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
        : ICommandHttpResponseMapper<InitiateOnboardingCommandResult, HttpContext, InitiateUserOnboardingResponse>
    {
        public IResult Map(
            InitiateOnboardingCommandResult commandResult, 
            HttpContext context,
            CommandHttpResponseGenerator<InitiateUserOnboardingResponse> commandHttpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => commandHttpResponseGenerator.Ok(new InitiateUserOnboardingResponse
                {
                    Id = ok.UserOnboardingId, 
                    ExternalId = ok.ExternalId, 
                    ExternalIdProvider = ok.ExternalIdProvider
                }),
                _ => commandHttpResponseGenerator.Conflict(
                    context,
                    "user_onboarding_concurrency_conflict",
                    "Could not initiate user onboarding because of a concurrency conflict. Please retry."));

    }
}
