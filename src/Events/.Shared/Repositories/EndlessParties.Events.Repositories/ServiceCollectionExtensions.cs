using EndlessParties.Events.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Repositories;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления репозиториев
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление репозиториев
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}