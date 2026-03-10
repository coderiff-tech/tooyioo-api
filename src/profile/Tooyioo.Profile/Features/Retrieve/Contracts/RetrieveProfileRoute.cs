using Microsoft.AspNetCore.Mvc;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Retrieve.Contracts;

public record RetrieveProfileRoute
{
    [FromRoute(Name = "id")] 
    public required Guid ProfileId { get; init; }

    // Add optional [FromQuery] parameters here
}