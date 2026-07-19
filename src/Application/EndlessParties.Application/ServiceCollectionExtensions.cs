using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Bookings.Services;
using EndlessParties.Application.Events.Services;
using EndlessParties.Shared.ChannelMessageBus;
using Microsoft.Extensions.DependencyInjection;

namespace EndlessParties.Application;

/// <summary>
/// Класс-расширение <see cref="IServiceCollection"/> для добавления сервисов прикладного слоя
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Добавление сервисов прикладного слоя
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddTransient<IEventService, EventService>()
            .AddTransient<IBookingService, BookingService>()
            .AddHostedService<BookingProcessorService>()
            .AddChannelMessageBus<BookingCreatedMessage>();

        return services;
    }
}