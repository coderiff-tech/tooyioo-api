using Microsoft.AspNetCore.Http;
using Slicent.Application;
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
        : IQueryHttpResponseMapper<RetrieveProfileQueryResult, HttpContext, RetrieveProfileResponse>
    {
        public IResult Map(
            RetrieveProfileQueryResult queryResult, 
            HttpContext context,
            QueryHttpResponseGenerator<RetrieveProfileResponse> queryHttpResponseGenerator)
            => queryResult.Match<IResult>(
                ok => queryHttpResponseGenerator.Ok(new RetrieveProfileResponse
                {
                    Id = ok.Document.Id,
                    IsComplete = ok.Document.IsComplete,
                    Alias = ok.Document.Alias,
                    Email = ok.Document.Email,
                    PhoneNumber = ok.Document.PhoneNumber,
                    CreatedAt = ok.Document.CreatedAt,
                    LastModifiedAt = ok.Document.LastModifiedAt,
                    CompletedAt = ok.Document.CompletedAt
                }),
                notFound => queryHttpResponseGenerator.NotFound(
                    context,
                    "profile_not_found",
                    $"Could not find Profile with Id {notFound.Id}"));
    }
}
