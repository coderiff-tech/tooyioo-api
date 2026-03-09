using Microsoft.AspNetCore.Mvc;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooiyoo.Identity.Features.CompleteProfile.Contracts;

public record CompleteIdentityProfileRoute
{
    [FromRoute(Name = "id")] 
    public Guid IdentityId { get; init; }
}