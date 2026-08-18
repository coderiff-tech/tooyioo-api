using System.Reflection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Tooyioo.Api.Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    private const string DocumentName = "public";
    private const string GoogleOAuthSecurityScheme = "GoogleOAuth";

    public static TBuilder AddOpenApi<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var googleOAuthAuthorizationUrl = 
            builder.Configuration.GetValue<string>("Scalar:GoogleOAuth:AuthorizationUrl")
            ?? throw new InvalidOperationException("Scalar:GoogleOAuth:AuthorizationUrl is not set");
        var googleOAuthTokenUrl =
            builder.Configuration.GetValue<string>("Scalar:GoogleOAuth:TokenUrl")
            ?? throw new InvalidOperationException("Scalar:GoogleOAuth:TokenUrl is not set");
        
        builder.Services.AddOpenApi(DocumentName, options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
            
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                
                document.Components.SecuritySchemes[GoogleOAuthSecurityScheme] =
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(googleOAuthAuthorizationUrl),
                                TokenUrl = new Uri(googleOAuthTokenUrl),
                                Scopes = new Dictionary<string, string>
                                {
                                    ["openid"] = "OpenID Connect",
                                    ["email"] = "Email",
                                    ["profile"] = "Profile"
                                }
                            }
                        }
                    };

                return Task.CompletedTask;
            });
                
            options.AddOperationTransformer((operation, context, _) =>
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [ new OpenApiSecuritySchemeReference(GoogleOAuthSecurityScheme, context.Document) ] = 
                    [
                        "openid",
                        "email",
                        "profile"
                    ]
                });

                return Task.CompletedTask;
            });
        });

        return builder;
    }

    public static WebApplication UseOpenApi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return app;
        }

        var googleOAuthClientId = 
            app.Configuration.GetValue<string>("Scalar:GoogleOAuth:ClientId")
            ?? throw new InvalidOperationException("Scalar:GoogleOAuth:ClientId is not set");
        var googleOAuthClientSecret = 
            app.Configuration.GetValue<string>("Scalar:GoogleOAuth:ClientSecret")
            ?? throw new InvalidOperationException("Scalar:GoogleOAuth:ClientSecret is not set");
        
        app.MapOpenApi().AllowAnonymous().CacheOutput();

        app.MapScalarApiReference(
                "/scalar",
                options =>
                    options
                        .ShowOperationId()
                        .SortTagsAlphabetically()
                        .WithOperationTitleSource(OperationTitleSource.Path)
                        .WithOpenApiRoutePattern($"/openapi/{DocumentName}.json")
                        .WithDocumentDownloadType(DocumentDownloadType.Json)
                        .AddPreferredSecuritySchemes(GoogleOAuthSecurityScheme)
                        .AddAuthorizationCodeFlow(GoogleOAuthSecurityScheme, flow =>
                        {
                            flow.ClientId = googleOAuthClientId;
                            flow.ClientSecret = googleOAuthClientSecret;

                            flow.Pkce = Pkce.Sha256;

                            flow.SelectedScopes =
                            [
                                "openid",
                                "email",
                                "profile"
                            ];

                            flow.RedirectUri =
                                "http://localhost:8000/scalar/oauth/callback";
                            
                            flow.TokenName = "id_token"; // Using id token, not the access token
                        }))
            .AllowAnonymous();

        return app;
    }
    
    private static ScalarOptions WithOperationTitleSource(
        this ScalarOptions options,
        OperationTitleSource source)
    {
        options.OperationTitleSource = source;
        return options;
    }

    public static bool IsOpenApiGenerationLaunch(this Assembly? entryAssembly)
        => entryAssembly?.GetName().Name == "GetDocument.Insider";
}