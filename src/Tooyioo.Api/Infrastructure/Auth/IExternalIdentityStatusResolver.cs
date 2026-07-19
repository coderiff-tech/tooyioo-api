using Tooyioo.Common;

namespace Tooyioo.Api.Infrastructure.Auth;

internal interface IExternalIdentityStatusResolver
{
    ValueTask<ExternalIdentityStatusResult> Resolve(
        ExternalIdentity externalIdentity,
        CancellationToken cancellationToken);
}