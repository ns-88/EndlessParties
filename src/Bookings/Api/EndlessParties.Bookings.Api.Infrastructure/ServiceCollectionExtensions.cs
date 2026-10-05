using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Repositories;
using EndlessParties.Shared.EventBus.Kafka;
using EndlessParties.Shared.Utils.DateTime;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Bookings.Api.Infrastructure;

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
            .AddTransient<IDateTimeProvider, DateTimeProvider>()
            .AddRepositories()
            .AddKafkaEventBus(settings.KafkaEventBus)
            .AddBookingsDatabase(settings.EventsDatabase);

        return services;
    }
}