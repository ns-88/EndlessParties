using EndlessParties.Events.Database;
using EndlessParties.Events.Domain.Messages;
using EndlessParties.Events.Repositories;
using EndlessParties.Shared.ChannelMessageBus;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Events.Api.Infrastructure;

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
            .AddRepositories()
            .AddChannelMessageBus<BookingCreatedMessage>()
            .AddEventsDatabase(settings.EventsDatabase);

        return services;
    }
}