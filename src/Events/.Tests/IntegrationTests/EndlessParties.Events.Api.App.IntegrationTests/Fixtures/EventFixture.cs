using EndlessParties.Events.Database.Database;
using EndlessParties.Events.Repositories;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Sdk;

namespace EndlessParties.Events.Api.App.IntegrationTests.Fixtures;

/// <summary>
/// Фикстура для работы с тестами обработчиков команд и запросов слоя Application
/// </summary>
public class EventFixture<THandler> : PostgreSqlContainerFixture<EventsDbContext> where THandler : class
{
    /// <inheritdoc />
    public EventFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <inheritdoc />
    protected override void ConfigureServices(ServiceCollection serviceCollection)
    {
        var eventBusMock = new Mock<IEventBus>();

        eventBusMock
            .Setup(x => x.Publish(It.IsAny<object>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        serviceCollection
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IEventBus>(_ => eventBusMock.Object)
            .AddScoped<THandler>();
    }
}