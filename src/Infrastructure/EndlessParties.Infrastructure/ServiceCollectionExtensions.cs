using EndlessParties.Database;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Infrastructure.Bookings.Repositories;
using EndlessParties.Infrastructure.Events.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Infrastructure;

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
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddEventsDatabase(settings.EventsDatabase);

        return services;
    }
}