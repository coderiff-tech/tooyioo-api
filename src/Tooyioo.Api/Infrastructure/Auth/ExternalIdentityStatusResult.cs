using Funzo;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Api.Infrastructure.Auth;

[Union<ExternalIdentityStatusUserOnboarding, ExternalIdentityStatusUserOnboarded, ExternalIdentityStatusUserUnknown>]
public partial class ExternalIdentityStatusResult;

public sealed record ExternalIdentityStatusUserOnboarding(string UserOnboardingId);

public sealed record ExternalIdentityStatusUserOnboarded(string UserId);
public sealed record ExternalIdentityStatusUserUnknown;