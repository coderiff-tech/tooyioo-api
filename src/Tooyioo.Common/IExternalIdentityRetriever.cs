using Microsoft.AspNetCore.Http;

namespace Tooyioo.Common;

public interface IExternalIdentityRetriever
{
    ExternalIdentity GetExternalIdentityFromContext(HttpContext context);
}