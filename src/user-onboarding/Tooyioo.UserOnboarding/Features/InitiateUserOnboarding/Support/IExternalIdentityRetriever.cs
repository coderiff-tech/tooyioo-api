using Microsoft.AspNetCore.Http;
using Tooyioo.Common;

namespace Tooyioo.UserOnboarding.Features.InitiateUserOnboarding.Support;

public interface IExternalIdentityRetriever
{
    ExternalIdentity GetExternalIdentityFromContext(HttpContext context);
}