using EndlessParties.Bookings.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Bookings.Repositories;

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
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}