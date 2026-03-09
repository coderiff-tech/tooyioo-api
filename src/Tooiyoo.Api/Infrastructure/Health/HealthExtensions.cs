using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tooiyoo.Api.Infrastructure.Health;

public static class HealthExtensions
{
    public const string HealthEndpointPath = "/health";
    public const string AlivenessEndpointPath = "/alive";

    extension<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddHealth()
        {
            builder.Services
                .AddServiceDiscovery()
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
            
            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication UseHealth()
        {
            app.MapHealthChecks(HealthEndpointPath);
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });

            return app;
        }
    }
}