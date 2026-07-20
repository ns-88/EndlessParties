using EndlessParties.Shared.MessageBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Shared.ChannelMessageBus;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления шины сообщений на основе каналов
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление шины сообщений на основе каналов
    /// </summary>
    public static IServiceCollection AddChannelMessageBus<TMessage>(this IServiceCollection services)
    {
        services
            .AddSingleton<ChannelMessageBus<TMessage>>()
            .AddSingleton<IPublisher<TMessage>>(x => x.GetRequiredService<ChannelMessageBus<TMessage>>())
            .AddSingleton<ISubscriber<TMessage>>(x => x.GetRequiredService<ChannelMessageBus<TMessage>>());

        return services;
    }
}