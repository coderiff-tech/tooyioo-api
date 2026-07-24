using Microsoft.AspNetCore.Mvc;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.User.Features.RetrieveUser.Contract;

public record RetrieveUserRoute
{
    [FromRoute(Name = "id")] 
    public required Guid UserId { get; init; }
}