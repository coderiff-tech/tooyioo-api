using Microsoft.AspNetCore.Http;

// ReSharper disable ClassNeverInstantiated.Global
namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

public interface IPersonalDetailsRetriever
{
    PersonalDetails GetPersonalDetailsFromContext(HttpContext context);
}

public sealed record PersonalDetails(string Name, string LastName, string Email, bool IsEmailVerified);