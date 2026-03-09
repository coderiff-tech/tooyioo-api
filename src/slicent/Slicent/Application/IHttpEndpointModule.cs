using Microsoft.AspNetCore.Routing;

namespace Slicent.Application;

public interface IHttpEndpointModule
{
    void Map(IEndpointRouteBuilder app);
}