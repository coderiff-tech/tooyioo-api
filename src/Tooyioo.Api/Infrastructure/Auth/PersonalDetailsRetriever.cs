using Tooyioo.Profile.Features.Bootstrap.Support;

namespace Tooyioo.Api.Infrastructure.Auth;

public class PersonalDetailsRetriever
    : IPersonalDetailsRetriever
{
    public PersonalDetails GetPersonalDetailsFromContext(HttpContext context)
        => HttpContextPersonalDetails.TryGet(context) ?? throw new InvalidOperationException("No personal details found in context");
}