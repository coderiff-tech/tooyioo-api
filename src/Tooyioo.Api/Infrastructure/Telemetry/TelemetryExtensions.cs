using System.Reflection;
using Eventuous.Diagnostics.OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Tooyioo.Api.Infrastructure.Health;
using Tooyioo.Api.Infrastructure.OpenApi;
// ReSharper disable UnusedType.Global
// ReSharper disable ConvertToExtensionBlock

namespace Tooyioo.Api.Infrastructure.Telemetry;

public static class TelemetryExtensions
{
    extension<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddTelemetry()
        {
            if (Assembly.GetEntryAssembly().IsOpenApiGenerationLaunch())
            {
                return builder;
            }

            var otelEnabled = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") != null;
            
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services
                .AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter("*")
                        .AddEventuous()
                        .AddEventuousSubscriptions();

                    if (otelEnabled)
                    {
                        metrics.AddOtlpExporter();
                    }
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(tr =>
                            tr.Filter = context =>
                                !context.Request.Path.StartsWithSegments(HealthExtensions.HealthEndpointPath)
                                && !context.Request.Path.StartsWithSegments(HealthExtensions.AlivenessEndpointPath)
                        )
                        .AddEventuousTracing()
                        .AddHttpClientInstrumentation()
                        .SetSampler(new AlwaysOnSampler());

                    if (otelEnabled)
                    {
                        tracing.AddOtlpExporter();
                    }
                });

            return builder;
        }
    }
}
