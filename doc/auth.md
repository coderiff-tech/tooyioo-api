# Auth

Authentication is host-level infrastructure. Authorization and identity-dependent mapping are part of the slice behavior when they affect a request.

## JWT claims

JWT claims should remain honest.

Do not mutate the identity token into something it is not. The token represents the external identity provider's claims.

For Google-style identity tokens, relevant claims include:

- `sub`
- `iss`
- `given_name`
- `family_name`
- `email`
- `email_verified`

Slices may read `ClaimsPrincipal` when the request behavior depends on authenticated identity.

## App-owned claims

The application may enrich the authenticated principal with app-owned claims after token validation.

Examples:

- known onboarding status
- known user id
- known user onboarding id

These claims are Tooyioo facts, not Google facts. Keep that distinction clear.

## Personal details

If personal details come from the identity token, parse them from the claims principal.

Do not introduce a separate personal-details retriever unless the source is genuinely different from the token, for example a separate profile API call, a privacy transform, or a provider-specific normalization flow.

## Tests

Tests should use real signed JWTs with a test signing key.

The vertical slice test host overrides JWT bearer options so tests do not depend on Google metadata, production issuer validation, or production audience validation.

Keep signing key validation and lifetime validation enabled in tests. This catches malformed token setup without making tests external.
