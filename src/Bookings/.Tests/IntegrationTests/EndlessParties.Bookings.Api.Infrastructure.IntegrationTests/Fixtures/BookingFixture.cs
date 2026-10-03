using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Repositories;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Sdk;

namespace EndlessParties.Bookings.Api.Infrastructure.IntegrationTests.Fixtures;

/// <summary>
/// Фикстура для работы с тестами репозитория <see cref="BookingRepository"/>
/// </summary>
public class BookingFixture : PostgreSqlContainerFixture<BookingsDbContext>
{
    /// <inheritdoc />
    public BookingFixture(IMessageSink messageSink) : base(messageSink)
    {
    }


    /// <inheritdoc />
    protected override void ConfigureServices(ServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBookingRepository, BookingRepository>();
    }
}