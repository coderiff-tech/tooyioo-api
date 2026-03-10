using Microsoft.AspNetCore.Http;
using Slicent.Application.Queries;
using Tooyioo.Profile.Domain;
using Tooyioo.Profile.Features.Retrieve.Contracts;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.Retrieve;

public static class RetrieveProfileMappers
{
    public sealed class QueryMapper
        : IQueryMapper<RetrieveProfileRoute, HttpContext, RetrieveProfileQuery>
    {
        public RetrieveProfileQuery Map(RetrieveProfileRoute urlParams, HttpContext context)
            => new(new ProfileId(urlParams.ProfileId.ToString()));
    }

    public sealed class QueryHttpResponseMapper
        : IQueryHttpResponseMapper<RetrieveProfileQueryResultOkResult, HttpContext, RetrieveProfileResponse>
    {
        public IResult Map(
            RetrieveProfileQueryResultOkResult queryResult, 
            HttpContext context,
            QueryHttpResponseGenerator<RetrieveProfileResponse> queryHttpResponseGenerator)
            => queryHttpResponseGenerator.Ok(new RetrieveProfileResponse
                {
                    Id = queryResult.ProfileId,
                    IsComplete = queryResult.IsComplete,
                    Alias = queryResult.Alias ?? string.Empty,
                    Email = queryResult.Email,
                    PhoneNumber = queryResult.PhoneNumber ?? string.Empty
                });
    }
}
