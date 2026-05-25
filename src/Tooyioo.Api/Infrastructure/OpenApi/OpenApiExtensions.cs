using System.Reflection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace Tooyioo.Api.Infrastructure.OpenApi;

public static class OpenApiExtensions
{
    private const string DocumentName = "public";
    
    extension<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddOpenApi()
        {
            builder.Services.AddOpenApi(DocumentName, options =>
            {
                options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
                const string securityScheme = "IdToken";
                options.AddDocumentTransformer((document, context, ct) =>
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
                
                options.AddOperationTransformer((operation, context, ct) =>
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
    }

    extension(WebApplication app)
    {
        public WebApplication UseOpenApi()
        {
            if (!app.Environment.IsDevelopment())
            {
                return app;
            }

            app.MapOpenApi().AllowAnonymous().CacheOutput();

            app.MapScalarApiReference(
                "/scalar",
                options => options
                    .WithOpenApiRoutePattern($"/openapi/{DocumentName}.json")
                    .WithDocumentDownloadType(DocumentDownloadType.Json))
                .AllowAnonymous();

            return app;
        }
    }

    extension(Assembly? entryAssembly)
    {
        public bool IsOpenApiGenerationLaunch()
            => entryAssembly?.GetName().Name == "GetDocument.Insider";
    }
}