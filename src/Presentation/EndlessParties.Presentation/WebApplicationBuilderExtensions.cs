using EndlessParties.Application;
using EndlessParties.Infrastructure;
using EndlessParties.Shared.Exceptions;
using EndlessParties.Shared.Utils.Database.Settings;
using EndlessParties.Shared.Validations;
using Serilog;

namespace EndlessParties.Presentation;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilder"/> для регистрации сервисов и конфигурации инфраструктуры приложения
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Конфигурирование сервисов
    /// </summary>
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder
            .AddSerilog()
            .AddApplicationExceptions()
            .AddApplicationValidations()
            .AddDependencyValidation();

        var infrastructureSettings = GetInfrastructureSettings(builder.Configuration);

        builder.Services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(infrastructureSettings);

        return builder;
    }

    /// <summary>
    /// Добавление проверки жизненного цикла и создания зависимостей
    /// </summary>
    private static void AddDependencyValidation(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsEnvironment("local"))
        {
            return;
        }

        builder.Host.UseDefaultServiceProvider(setup =>
        {
            setup.ValidateScopes = true;
            setup.ValidateOnBuild = true;
        });
    }

    /// <summary>
    /// Добавление логгера Serilog
    /// </summary>
    private static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services);
        });

        return builder;
    }

    /// <summary>
    /// Получение настроек <see cref="InfrastructureSettings"/>
    /// </summary>
    private static InfrastructureSettings GetInfrastructureSettings(IConfiguration config)
    {
        return new InfrastructureSettings
        {
            EventsDatabase = new DatabaseSettings
            {
                ConnectionString = config.GetConnectionString("Postgres:Events")!,
                RetryReconnectDatabaseCount = int.Parse(config["Database:RetryOnFailureCount"]!)
            }
        };
    }
}