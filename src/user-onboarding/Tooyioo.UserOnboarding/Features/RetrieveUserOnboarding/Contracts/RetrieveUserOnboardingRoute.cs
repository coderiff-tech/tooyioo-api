using Microsoft.AspNetCore.Mvc;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding.Contracts;

public record RetrieveUserOnboardingRoute
{
    [FromRoute(Name = "id")] 
    public required Guid UserOnboardingId { get; init; }
}