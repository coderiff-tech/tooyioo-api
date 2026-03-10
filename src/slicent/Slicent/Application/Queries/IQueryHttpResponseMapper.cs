using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Queries;

public interface IQueryHttpResponseMapper<in TQueryResult, in TContext, THttpResponse>
    where THttpResponse : class
{
    IResult Map(TQueryResult queryResult, TContext context, QueryHttpResponseGenerator<THttpResponse> queryHttpResponseGenerator);
}