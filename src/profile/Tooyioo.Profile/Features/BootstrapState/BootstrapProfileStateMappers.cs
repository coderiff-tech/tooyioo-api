using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Slicent.Application.Commands;
using Tooyioo.Common;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Features.Bootstrap.Contracts;
using Tooyioo.UserOnboarding.Features.Initiate.Support;

// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.BootstrapState;

public static class BootstrapProfileStateMappers
{
    public sealed class CommandMapper
        : ICommandMapper<BootstrapProfileRequest, HttpContext, BootstrapProfileStateCommand>
    {
        private readonly IPersonalDetailsRetriever _personalDetailsRetriever;

        public CommandMapper(
            IPersonalDetailsRetriever personalDetailsRetriever,
            ILogger<CommandMapper> logger)
        {
            _personalDetailsRetriever = personalDetailsRetriever;
        }

        public BootstrapProfileStateCommand Map(BootstrapProfileRequest request, HttpContext context)
        {
            var personalDetails = _personalDetailsRetriever.GetPersonalDetailsFromContext(context);

            var subOption = context.User.GetSubClaim();
            var externalId = subOption.ValueOr(string.Empty);

            return new BootstrapProfileStateCommand(
                ProfileId.New(),
                personalDetails.Name.Trim(),
                personalDetails.LastName.Trim(),
                personalDetails.Email.Trim(),
                personalDetails.IsEmailVerified,
                externalId.Trim(),
                "Google");
        }
    }

    public sealed class CommandHttpResponseMapper
        : ICommandHttpResponseMapper<BootstrapProfileStateCommandResult, HttpContext, BootstrapProfileResponse>
    {
        public IResult Map(
            BootstrapProfileStateCommandResult commandResult,
            HttpContext context,
            CommandHttpResponseGenerator<BootstrapProfileResponse> commandHttpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => commandHttpResponseGenerator.Ok(new BootstrapProfileResponse
                {
                    Id = ok.ProfileId,
                    ExternalId = ok.ExternalId,
                    ExternalIdProvider = ok.ExternalIdProvider
                }),
                _ => commandHttpResponseGenerator.Conflict(
                    context,
                    "profile_concurrency_conflict",
                    "Could not bootstrap the Profile because of a concurrency conflict. Please retry."));
    }
}
