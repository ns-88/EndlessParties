using EndlessParties.Identity.Database.Database;
using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.Database.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Identity.Database;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления базы данных "Identity"
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление базы данных "Identity"
    /// </summary>
    public static IServiceCollection AddIdentityDatabase(this IServiceCollection services, DatabaseSettings settings)
    {
        services.AddDatabase<IdentityDbContext>(settings);

        return services;
    }
}