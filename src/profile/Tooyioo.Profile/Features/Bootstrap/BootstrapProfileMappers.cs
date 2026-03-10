using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Slicent.Application.Commands;
using Tooiyoo.Common;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Features.Bootstrap.Contracts;
using Tooyioo.Profile.Features.Bootstrap.Support;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Bootstrap;

public static class BootstrapProfileMappers
{
    public sealed class CommandMapper
        : ICommandMapper<BootstrapProfileRequest, HttpContext, BootstrapProfileCommand>
    {
        private readonly IPersonalDetailsRetriever _personalDetailsRetriever;

        public CommandMapper(
            IPersonalDetailsRetriever personalDetailsRetriever,
            ILogger<CommandMapper> logger)
        {
            _personalDetailsRetriever = personalDetailsRetriever;
        }
        
        public BootstrapProfileCommand Map(BootstrapProfileRequest request, HttpContext context)
        {
            var personalDetails = _personalDetailsRetriever.GetPersonalDetailsFromContext(context);

            var subOption = context.User.GetSubClaim();
            var externalId = subOption.ValueOr(string.Empty);

            return new BootstrapProfileCommand(
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
        : ICommandHttpResponseMapper<BootstrapProfileCommandResult, HttpContext, BootstrapProfileResponse>
    {
        public IResult Map(
            BootstrapProfileCommandResult commandResult, 
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
                    "identity_concurrency_conflict",
                    "Could not bootstrap the Identity because of a concurrency conflict. Please retry."));
    }
}
