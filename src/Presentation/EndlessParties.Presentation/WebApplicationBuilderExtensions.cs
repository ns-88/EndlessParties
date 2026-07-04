using EndlessParties.Application;
using EndlessParties.Infrastructure;
using EndlessParties.Shared.Exceptions;
using EndlessParties.Shared.Validations;
using Serilog;

namespace EndlessParties.Presentation;

/// <summary>
/// Класс-расширение <see cref="WebApplicationBuilder"/> для регистрации сервисов и конфигурации инфраструктуры приложения
/// </summary>
public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        /// Конфигурирование сервисов
        /// </summary>
        public WebApplicationBuilder ConfigureServices()
        {
            builder
                .AddSerilog()
                .AddApplicationExceptions()
                .AddApplicationValidations()
                .AddDependencyValidation();

            builder.Services
                .AddPresentation()
                .AddApplication()
                .AddInfrastructure();

            return builder;
        }

        /// <summary>
        /// Добавление проверки жизненного цикла и создания зависимостей
        /// </summary>
        private void AddDependencyValidation()
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
        private WebApplicationBuilder AddSerilog()
        {
            builder.Services.AddSerilog((services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services);
            });

            return builder;
        }
    }
}