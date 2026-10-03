using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Bookings.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления базы данных "Bookings"
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных "Bookings"
    /// </summary>
    public static IServiceCollection AddBookingsDatabase(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddDatabase<BookingsDbContext>(settings);

        return services;
    }
}