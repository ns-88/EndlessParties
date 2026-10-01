using EndlessParties.Identity.Api.App;
using EndlessParties.Identity.Api.Infrastructure;
using EndlessParties.Identity.Cryptography.Settings;
using EndlessParties.Shared.Exceptions;
using EndlessParties.Shared.Utils.Database.Settings;
using EndlessParties.Shared.Utils.Logger;
using EndlessParties.Shared.Validations;

namespace EndlessParties.Identity.Api;

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
    /// Получение настроек <see cref="InfrastructureSettings"/>
    /// </summary>
    private static InfrastructureSettings GetInfrastructureSettings(IConfiguration config)
    {
        return new InfrastructureSettings
        {
            IdentityDatabase = new DatabaseSettings
            {
                ConnectionString = config.GetConnectionString("Postgres:Identity")!,
                RetryReconnectDatabaseCount = int.Parse(config["Database:RetryOnFailureCount"]!)
            },
            JwtToken = config
                .GetRequiredSection("JwtToken")
                .Get<JwtTokenSettings>()!
        };
    }
}