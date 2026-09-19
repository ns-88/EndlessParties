using EndlessParties.Events.Database.Database;
using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления базы данных "Events"
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных "Events"
    /// </summary>
    public static IServiceCollection AddEventsDatabase(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddDatabase<EventsDbContext>(settings);

        return services;
    }
}