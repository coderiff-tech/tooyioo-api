using System.Reflection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Tooyioo.Api.Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    private const string DocumentName = "public";

    public static TBuilder AddOpenApi<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddOpenApi(DocumentName, options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
            const string securityScheme = "IdToken";
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                document.Components.SecuritySchemes[securityScheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Send a Google OpenID Connect id_token as: Authorization: Bearer <id_token>"
                };

                return Task.CompletedTask;
            });
                
            options.AddOperationTransformer((operation, context, _) =>
            {
                operation.Security ??= new List<OpenApiSecurityRequirement>();

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [ new OpenApiSecuritySchemeReference(securityScheme, context.Document) ] = []
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

        app.MapOpenApi().AllowAnonymous().CacheOutput();

        app.MapScalarApiReference(
                "/scalar",
                options =>
                    options
                        .ShowOperationId()
                        .SortTagsAlphabetically()
                        .WithOperationTitleSource(OperationTitleSource.Path)
                        .WithOpenApiRoutePattern($"/openapi/{DocumentName}.json")
                        .WithDocumentDownloadType(DocumentDownloadType.Json))
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