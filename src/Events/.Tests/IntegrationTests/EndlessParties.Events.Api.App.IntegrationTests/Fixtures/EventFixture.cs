using EndlessParties.Events.Database.Database;
using EndlessParties.Events.Repositories;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
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
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var userContextAccessorMock = new Mock<IUserContextAccessor>();

        eventBusMock
            .Setup(x => x.Publish(It.IsAny<object>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        dateTimeProviderMock
            .Setup(x => x.UtcNow())
            .Returns(TestConstants.StartAt.AddDays(-1));

        userContextAccessorMock
            .SetupGet(x => x.Current)
            .Returns(TestConstants.AdminUserContext);

        serviceCollection
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IEventBus>(_ => eventBusMock.Object)
            .AddScoped<IDateTimeProvider>(_ => dateTimeProviderMock.Object)
            .AddScoped<IUserContextAccessor>(_ => userContextAccessorMock.Object)
            .AddScoped<THandler>();
    }
}