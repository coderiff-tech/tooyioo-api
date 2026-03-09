using Tooiyoo.Identity.Features.Bootstrap.Support;

namespace Tooiyoo.Api.Infrastructure.Auth;

public class PersonalDetailsRetriever
    : IPersonalDetailsRetriever
{
    public PersonalDetails GetPersonalDetailsFromContext(HttpContext context)
        => HttpContextPersonalDetails.TryGet(context) ?? throw new InvalidOperationException("No personal details found in context");
}