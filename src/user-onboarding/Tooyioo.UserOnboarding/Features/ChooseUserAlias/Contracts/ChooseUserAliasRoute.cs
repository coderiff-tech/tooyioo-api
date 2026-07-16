using Microsoft.AspNetCore.Mvc;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.UserOnboarding.Features.ChooseUserAlias.Contracts;

public record ChooseUserAliasRoute
{
    [FromRoute(Name = "id")] 
    public required Guid UserOnboardingId { get; init; }
}