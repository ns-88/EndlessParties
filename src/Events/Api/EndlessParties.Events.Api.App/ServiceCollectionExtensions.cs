using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Api.App;

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
            setup.Namespace = "EndlessParties.Events.Api.App.Mediator";
            setup.ServiceLifetime = ServiceLifetime.Scoped;
            setup.GenerateTypesAsInternal = true;
            setup.Assemblies = [typeof(ServiceCollectionExtensions).Assembly];
        });

        return services;
    }
}