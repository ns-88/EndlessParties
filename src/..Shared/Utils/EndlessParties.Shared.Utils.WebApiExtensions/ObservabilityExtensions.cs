using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления и настройки компонентов наблюдаемости
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// Добавление и настройка компонентов наблюдаемости
    /// </summary>
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        var settings = builder.GetObservabilitySettings();

        builder.Services
            .AddTransient<IStartupFilter, MetricsStartupFilter>()
            .AddOpenTelemetry()
            .ConfigureResource(resourceSetup => resourceSetup.AddService(settings.ServiceName))
            .WithMetrics(metricsSetup =>
            {
                metricsSetup
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter(prometheusSetup =>
                    {
                        prometheusSetup.ResourceConstantLabels = attributeKey => attributeKey == "service.name";
                    });
            })
            .WithTracing(tracingSetup =>
            {
                tracingSetup
                    .AddAspNetCoreInstrumentation(aspNetCoreSetup =>
                    {
                        aspNetCoreSetup.Filter = httpContext => !httpContext.Request.Path.StartsWithSegments("/metrics") &&
                                                                !httpContext.Request.Path.StartsWithSegments("/healthcheck/health") &&
                                                                !httpContext.Request.Path.StartsWithSegments("/api/health");
                    })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(efCoreSetup =>
                    {
                        efCoreSetup.Filter = (_, command) => !command.CommandText.Contains("__EFMigrationsHistory");
                    })
                    .AddOtlpExporter(otlpSetup =>
                    {
                        otlpSetup.Protocol = OtlpExportProtocol.Grpc;
                        otlpSetup.Endpoint = new Uri(settings.OtlpEndpoint);
                    });
            });

        return builder;
    }

    /// <summary>
    /// Получение настроек <see cref="ObservabilitySettings"/>
    /// </summary>
    private static ObservabilitySettings GetObservabilitySettings(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetRequiredSection("Observability")
            .Get<ObservabilitySettings>()!;

        settings.Validate();

        return settings;
    }
}