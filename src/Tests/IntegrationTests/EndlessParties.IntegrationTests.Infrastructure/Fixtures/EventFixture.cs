using EndlessParties.Database.Database;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Infrastructure.Bookings.Repositories;
using EndlessParties.Infrastructure.Events.Repositories;
using EndlessParties.Shared.Utils.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace EndlessParties.IntegrationTests.Infrastructure.Fixtures;

/// <summary>
/// Фикстура для работы с тестами репозиториев <see cref="EventRepository"/> и <see cref="BookingRepository"/>
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
            .AddScoped<IBookingRepository, BookingRepository>();
    }
}