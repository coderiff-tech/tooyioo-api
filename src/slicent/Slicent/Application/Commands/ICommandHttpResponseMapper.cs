using Microsoft.AspNetCore.Http;

namespace Slicent.Application.Commands;

public interface ICommandHttpResponseMapper<in TCommandResult, in TContext, THttpResponse>
    where THttpResponse : class
{
    IResult Map(TCommandResult commandResult, TContext context, CommandHttpResponseGenerator<THttpResponse> commandHttpResponseGenerator);
}