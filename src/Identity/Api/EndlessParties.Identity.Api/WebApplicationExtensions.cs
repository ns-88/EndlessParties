using EndlessParties.Identity.Database.Database;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Identity.Api;

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
        application
            .ApplyMigrations();

        return application;
    }

    /// <summary>
    /// Добавление и настройка Swagger
    /// </summary>
    private static WebApplication AddSwagger(this WebApplication application)
    {
        if (!application.Environment.IsEnvironment("local"))
        {
            return application;
        }

        application
            .UseSwagger()
            .UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity API V1");
                x.RoutePrefix = string.Empty;
            });

        return application;
    }

    /// <summary>
    /// Применение миграций
    /// </summary>
    private static void ApplyMigrations(this WebApplication application)
    {
        if (!application.Environment.IsEnvironment("local"))
        {
            return;
        }

        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        dbContext.Database.Migrate();
    }
}