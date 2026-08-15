using EndlessParties.Database.Database;
using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления базы данных событий (мероприятий)
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных событий (мероприятий)
    /// </summary>
    public static IServiceCollection AddEventsDatabase(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddDatabase<EventsDbContext>(settings);

        return services;
    }
}