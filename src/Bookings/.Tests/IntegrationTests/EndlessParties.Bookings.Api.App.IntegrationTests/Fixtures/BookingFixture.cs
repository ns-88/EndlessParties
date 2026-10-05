using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Repositories;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit.Sdk;

namespace EndlessParties.Bookings.Api.App.IntegrationTests.Fixtures;

using static TestConstants;

/// <summary>
/// Фикстура для работы с тестами обработчиков команд и запросов слоя Application
/// </summary>
public class BookingFixture<THandler> : PostgreSqlContainerFixture<BookingsDbContext> where THandler : class
{
    /// <inheritdoc />
    public BookingFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <inheritdoc />
    protected override void ConfigureServices(ServiceCollection serviceCollection)
    {
        var userContextAccessorMock = new Mock<IUserContextAccessor>();

        userContextAccessorMock
            .SetupGet(x => x.Current)
            .Returns(AdminUserContext);

        serviceCollection
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IUserContextAccessor>(_ => userContextAccessorMock.Object)
            .AddScoped<THandler>();
    }
}