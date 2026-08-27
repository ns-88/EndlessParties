using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Bookings.Services;
using EndlessParties.Application.Events.Services;
using EndlessParties.Database.Database;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Infrastructure.Bookings.Repositories;
using EndlessParties.Infrastructure.Events.Repositories;
using EndlessParties.Shared.ChannelMessageBus;
using EndlessParties.Shared.Utils.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace EndlessParties.IntegrationTests.Application.Fixtures;

/// <summary>
/// Фикстура для работы с тестами сервисов <see cref="EventService"/> и <see cref="BookingService"/>
/// </summary>
public class EventFixture : PostgreSqlContainerFixture<EventsDbContext>
{
    /// <inheritdoc />
    public EventFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <inheritdoc />
    protected override void ConfigureServices(ServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IBookingService, BookingService>()
            .AddChannelMessageBus<BookingCreatedMessage>();
    }
}