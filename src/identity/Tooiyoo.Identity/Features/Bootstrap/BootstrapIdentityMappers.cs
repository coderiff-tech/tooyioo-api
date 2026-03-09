using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Slicent.Application;
using Tooiyoo.Common;
using Tooiyoo.Identity.Domain;
using Tooiyoo.Identity.Features.Bootstrap.Contracts;
using Tooiyoo.Identity.Features.Bootstrap.Support;
// ReSharper disable ConvertToPrimaryConstructor

// ReSharper disable UnusedType.Global

namespace Tooiyoo.Identity.Features.Bootstrap;

public static class BootstrapIdentityMappers
{
    public sealed class BootstrapIdentityCommandMapper
        : ICommandMapper<BootstrapIdentityRequest, HttpContext, BootstrapIdentityCommand>
    {
        private readonly IPersonalDetailsRetriever _personalDetailsRetriever;
        private readonly ILogger<BootstrapIdentityCommandMapper> _logger;

        public BootstrapIdentityCommandMapper(
            IPersonalDetailsRetriever personalDetailsRetriever,
            ILogger<BootstrapIdentityCommandMapper> logger)
        {
            _personalDetailsRetriever = personalDetailsRetriever;
            _logger = logger;
        }
        
        public BootstrapIdentityCommand Map(BootstrapIdentityRequest request, HttpContext context)
        {
            var personalDetails = _personalDetailsRetriever.GetPersonalDetailsFromContext(context);

            var subOption = context.User.GetSubClaim();
            var externalId = subOption.ValueOr(string.Empty);

            return new BootstrapIdentityCommand(
                IdentityId.New(),
                personalDetails.Name.Trim(),
                personalDetails.LastName.Trim(),
                personalDetails.Email.Trim(),
                personalDetails.IsEmailVerified,
                externalId.Trim(),
                "Google");
        }
    }

    public sealed class BootstrapIdentityCommandHttpResponseMapper
        : ICommandHttpResponseMapper<BootstrapIdentityCommandResult, HttpContext, BootstrapIdentityResponse>
    {
        public IResult Map(
            BootstrapIdentityCommandResult commandResult, 
            HttpContext context,
            CommandHttpResponseGenerator<BootstrapIdentityResponse> httpResponseGenerator)
            => commandResult.Match<IResult>(
                ok => httpResponseGenerator.Ok(new BootstrapIdentityResponse
                {
                    Id = ok.IdentityId, 
                    ExternalId = ok.ExternalId, 
                    ExternalIdentityProvider = ok.ExternalIdentityProvider
                }),
                _ => httpResponseGenerator.Conflict(
                    context,
                    "identity_concurrency_conflict",
                    "Could not bootstrap the Identity because of a concurrency conflict. Please retry."));
    }
}
