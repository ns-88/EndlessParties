using EndlessParties.Events.Database;
using EndlessParties.Events.Repositories;
using EndlessParties.Shared.EventBus.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Worker.Infrastructure;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов инфраструктурного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов инфраструктурного слоя
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, InfrastructureSettings settings)
    {
        settings.Validate();

        services
            .AddRepositories()
            .AddKafkaEventBus(settings.KafkaEventBus)
            .AddEventsDatabase(settings.EventsDatabase);

        return services;
    }
}