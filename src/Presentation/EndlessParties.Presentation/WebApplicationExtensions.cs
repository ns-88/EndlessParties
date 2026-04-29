using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace EndlessParties.Presentation;

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
            .AddSwagger()
            .UseRouting();

        application
            .MapHealthChecks("/healthcheck/health", new HealthCheckOptions { AllowCachingResponses = false })
            .ExcludeFromDescription();

        application
            .MapGet("/healthcheck/ready", () => "Ready")
            .ExcludeFromDescription();

        application
            .MapControllers();

        return application;
    }

    /// <summary>
    /// Добавление и настройка Swagger
    /// </summary>
    private static WebApplication AddSwagger(this WebApplication application)
    {
        if (application.Environment.IsEnvironment("local"))
        {
            application
                .UseSwagger()
                .UseSwaggerUI(x =>
                {
                    x.SwaggerEndpoint("/swagger/v1/swagger.json", "EndlessParties API V1");
                    x.RoutePrefix = string.Empty;
                });
        }

        return application;
    }
}