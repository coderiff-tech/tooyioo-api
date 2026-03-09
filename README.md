# tooyioo-api
Event Source application with vertical slice approach

## Pre-requirements
- .NET 10 SDK
- Docker

## Getting Started
Run the Aspire's AppHost application, which will start the KurrentDB and MongoDB containers and will also initiate the Api main project. 
Access `http://localhost:2113` to explore KurrentDB
Access `http://localhost:27018` to explore MongoDB with Mongo Express

## Test & Report
Run
```
dotnet tool restore
```
to restore the reportgenerator tool

Run 
```
dotnet test --coverage --report-trx
```
to execute all tests with coverage and report trx from the slnx.

Run
```
dotnet tool run reportgenerator \
  -reports:**/TestResults/*.coverage \
  -targetdir:report \
  -reporttypes:Html
```
to create html report accessible from `report/index.html`.

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

### Google Project
Existing project in [Google Cloud Console](https://console.cloud.google.com/) called `tooyioo` that contains all necessary resources to use Google Sign-Up and Google Sign-In.
Steps are described in [this article](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-10.0).
The OAuth client is `tooyioo-web`

#### Steps to test
1. Create a new project in Google Cloud Console if it does not exist. Take note of the clientId and secret.
2. Ensure the Authorized redirect URIs is set to `https://developers.google.com/oauthplayground`
3. Open https://developers.google.com/oauthplayground/
4. On the upper right corner, select the `Use your own OAuth credentials` option and enter your client Id and secret.
5. Specify scopes openid email profile phone
6. Click Authorize APIs and then Exchange authorization code for tokens.
7. View the `id_token` and use it as Bearer token in the Authorization header. We don't need the access token.
8. Test the refresh token functionality.

For Bruno ensure `Use System Browser for OAuth2 Authorization` is enabled in Preferences > General