using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Cache.Redis.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.Cache.Redis;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления клиента СУБД Redis
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление клиента СУБД Redis
    /// </summary>
    public static IServiceCollection AddRedisClient(this IServiceCollection services, RedisClientSettings settings)
    {
        settings.Validate();

        var connectionProvider = new RedisConnectionProvider(settings);

        services
            .AddSingleton<IRedisConnectionProvider>(connectionProvider)
            .AddSingleton(connectionProvider)
            .AddScoped<ICacheClient, RedisCacheClient>()
            .AddHostedService<RedisLifecycleManager>();

        return services;
    }
}