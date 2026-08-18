# tooyioo-api
Event Source application with vertical slice approach

## Pre-requirements
- .NET 10 SDK
- Podman 6.1.0+
- Access to the `tooyioo` project in [Google Cloud Console](https://console.cloud.google.com/)

## Getting Started

### 1. Configure Google OAuth for Scalar
The API uses the `tooyioo-web` OAuth client from the `tooyioo` Google Cloud project.

The Google Client ID is already configured in `appsettings.json` and is used by the API to validate Google ID tokens.

Scalar also uses this OAuth client to authenticate with Google when testing the API locally. This requires the OAuth client secret, which must not be committed to source control.

From the `src/Tooyioo.Api` directory, configure the client secret using .NET User Secrets:

```bash
dotnet user-secrets set "Scalar:GoogleOAuth:ClientSecret" "<google-oauth-client-secret>"
```

The secret can be obtained from the tooyioo-web OAuth client in Google Cloud Console.
Verify your local configuration with:
```bash
dotnet user-secrets list
```

The tooyioo-web OAuth client must also contain the following Authorized redirect URI:
```bash
http://localhost:8000/scalar/oauth/callback
```

### 2. Start Podman
Start the Podman machine:
```bash
podman machine start
```

Verify that Podman is running:
```bash
podman info
```

### 3. Start the application
Run the Aspire AppHost project.
Aspire will start:
- Tooyioo API
- KurrentDB
- MongoDB
- Mongo Express

### 4. Access local services
Once Aspire has started the application:
- Scalar API Reference: http://localhost:8000/scalar
- OpenAPI document: http://localhost:8000/openapi/public.json
- KurrentDB: http://localhost:2113
- Mongo Express: http://localhost:27018

### 5. Access local services
Open:
```bash
http://localhost:8000/scalar
```

Use the authentication controls to sign in with Google.
Scalar performs the Google OAuth Authorization Code flow using the scopes `openid email profile`

Google returns both an OAuth access token and an OpenID Connect ID token.
The API authenticates requests using the Google `id_token`, not the OAuth `access_token`.
Once authenticated, requests made from Scalar to protected endpoints include:
```bash
Authorization: Bearer <id_token>
```

## Test & Report
Run:
```bash
dotnet tool restore
```
to restore the reportgenerator tool

Run:
```bash
dotnet test --coverage --report-trx
```
to execute all tests with coverage and report trx from the slnx.

Run:
```
dotnet tool run reportgenerator \
  -reports:**/TestResults/*.coverage \
  -targetdir:report \
  -reporttypes:Html
```
to create html report accessible from `report/index.html`.

## Interacting with the API

### Scalar

Scalar is the recommended way to interactively explore and test individual API endpoints during development.

Once the application is running and the Google OAuth secret has been configured as described in **Getting Started**, access Scalar at:

`http://localhost:8000/scalar`

Scalar handles Google authentication and automatically uses the resulting `id_token` as the Bearer token for authenticated requests.

### Bruno

Bruno is recommended for more complex or repeatable scenarios, such as chaining requests or reusing values from previous responses.

Ensure `Use System Browser for OAuth2 Authorization` is enabled under `Preferences > General`.

The collection reads the Google ID token directly from Bruno's OAuth store using:

`{{$oauth2.google-credentials.id_token}}`

## Gotchas

### Note on Minimal API validation in multi-assembly setups

When using ASP.NET Minimal APIs built-in validation with endpoints and request DTOs defined in different assemblies (or endpoints mapped indirectly via shared generic helpers), two extra steps are currently required due to limitations in the validation source generator:

Annotate request DTOs with `[ValidatableType]`
This forces the validation source generator to include the DTO even when it does not appear directly in a Minimal API handler signature.

Include an `AddValidation()` “trigger” in the DTO/feature assembly
A non-referenced helper like `ValidationCodegenTrigger` is required so the validation source generator runs for that assembly and emits its validation metadata.

This is a known limitation of the current Minimal API built-in validation when endpoints and DTOs live in separate assemblies.

Details and rationale are explained here:
https://stackoverflow.com/q/79855763/2948212
