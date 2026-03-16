using Microsoft.AspNetCore.Http;
using Slicent.Application;
using Slicent.Application.Queries;
using Tooyioo.Profile.Features.RetrieveAll.Contracts;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global

namespace Tooyioo.Profile.Features.RetrieveAll;

public static class RetrieveAllProfileMappers
{
    public sealed class QueryMapper
        : IQueryMapper<RetrieveAllProfileRoute, HttpContext, RetrieveAllProfileQuery>
    {
        public RetrieveAllProfileQuery Map(RetrieveAllProfileRoute urlParams, HttpContext context)
            => new()
            {
                Alias = urlParams.Alias,
                IsComplete = urlParams.IsComplete,
                PageNumber = urlParams.PageNumber ?? 1,
                PageSize = urlParams.PageSize ?? 20,
                CreatedAtFrom = urlParams.CreatedAtFrom,
                CreatedAtTo = urlParams.CreatedAtTo,
                LastModifiedAtFrom = urlParams.LastModifiedAtFrom,
                LastModifiedAtTo = urlParams.LastModifiedAtTo,
                CompletedAtFrom = urlParams.CompletedAtFrom,
                CompletedAtTo = urlParams.CompletedAtTo
            };
    }

    public sealed class QueryHttpResponseMapper
        : IQueryHttpResponseMapper<RetrieveAllProfileQueryResult, HttpContext, RetrieveAllProfileResponse>
    {
        public IResult Map(
            RetrieveAllProfileQueryResult queryResult, 
            HttpContext context,
            QueryHttpResponseGenerator<RetrieveAllProfileResponse> queryHttpResponseGenerator)
        {
            var pagedResult = queryResult.PagedResult;

            return queryHttpResponseGenerator.Ok(new RetrieveAllProfileResponse
            {
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                TotalCount = pagedResult.TotalCount,
                Items = pagedResult.Items.Select(x
                    => new RetrieveAllProfileResponseItem
                    {
                        Id = x.Id,
                        Alias = x.Alias,
                        CreatedAt = x.CreatedAt,
                        LastModifiedAt = x.LastModifiedAt,
                        IsComplete = x.IsComplete,
                        CompletedAt = x.CompletedAt
                    })
            });
        }
    }
}
