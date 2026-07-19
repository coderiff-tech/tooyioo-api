namespace Tooyioo.Common;

public sealed record ExternalIdentity(string Id, ExternalIdentityProvider Provider, string Issuer);