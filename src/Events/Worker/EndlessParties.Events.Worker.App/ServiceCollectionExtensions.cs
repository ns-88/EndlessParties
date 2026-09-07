using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Worker.App;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов прикладного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов прикладного слоя
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(setup =>
        {
            setup.Namespace = "EndlessParties.Events.Worker.App.Mediator";
            setup.ServiceLifetime = ServiceLifetime.Singleton;
            setup.GenerateTypesAsInternal = true;
            setup.Assemblies = [typeof(ServiceCollectionExtensions).Assembly];
        });

        return services;
    }
}