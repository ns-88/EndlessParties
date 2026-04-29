using EndlessParties.Application;
using EndlessParties.Infrastructure;

namespace EndlessParties.Presentation
{
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
            builder.Services
                .AddPresentation()
                .AddApplication()
                .AddInfrastructure();

            builder
                .AddDependencyValidation();

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
    }
}