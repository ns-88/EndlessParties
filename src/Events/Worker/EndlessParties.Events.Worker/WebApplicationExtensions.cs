using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace EndlessParties.Events.Worker;

/// <summary>
/// Класс-расширение <see cref="WebApplication"/> для конфигурации приложения
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Конфигурирование приложения
    /// </summary>
    public static WebApplication ConfigureApp(this WebApplication application)
    {
        application
            .MapHealthChecks("/healthcheck/health", new HealthCheckOptions { AllowCachingResponses = false })
            .ExcludeFromDescription();

        application
            .MapGet("/healthcheck/ready", () => "Ready")
            .ExcludeFromDescription();

        return application;
    }
}