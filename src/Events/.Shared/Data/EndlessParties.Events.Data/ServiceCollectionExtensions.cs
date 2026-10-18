using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Shared.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Data;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления провайдеров данных
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление провайдеров данных
    /// </summary>
    public static IServiceCollection AddDataProviders(this IServiceCollection services)
    {
        services
            .AddScoped<EventDataProvider>()
            .AddScoped<IEventDataProvider, CachingEventDataProvider>(sp =>
            {
                var eventRepository = sp.GetRequiredService<EventDataProvider>();
                var cacheClient = sp.GetRequiredService<ICacheClient>();

                return new CachingEventDataProvider(eventRepository, cacheClient);
            });

        return services;
    }
}