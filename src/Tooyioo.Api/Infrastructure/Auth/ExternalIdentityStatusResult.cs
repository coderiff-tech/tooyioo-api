using Funzo;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Api.Infrastructure.Auth;

[Union<ExternalIdentityStatusUserOnboarding, ExternalIdentityStatusUserOnboarded, ExternalIdentityStatusUserUnknown>]
public partial class ExternalIdentityStatusResult;

public sealed record ExternalIdentityStatusUserOnboarding(string UserOnboardingId);

public sealed record ExternalIdentityStatusUserOnboarded(string UserOnboardingId, string UserId);
public sealed record ExternalIdentityStatusUserUnknown;
