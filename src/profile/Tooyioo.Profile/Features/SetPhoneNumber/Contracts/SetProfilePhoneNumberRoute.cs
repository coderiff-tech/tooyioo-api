using Microsoft.AspNetCore.Mvc;

// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.SetPhoneNumber.Contracts;

public record SetProfilePhoneNumberRoute
{
    [FromRoute(Name = "id")] 
    public required Guid ProfileId { get; init; }
}