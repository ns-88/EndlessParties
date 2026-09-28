using AutoFixture;
using EndlessParties.Events.Api.App.Features.Bookings.Create;
using EndlessParties.Events.Api.App.Features.Bookings.GetById;
using EndlessParties.Events.Api.App.IntegrationTests.Fixtures;
using EndlessParties.Events.Database.Database;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.Events.Api.App.IntegrationTests.Features.Bookings.GetById;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="GetBookingByIdHandler"/>
/// </summary>
[Trait("Category", "Integration")]
public class GetByIdHandlerTests : BaseIntegrationTest<EventsDbContext, EventFixture<GetBookingByIdHandler>>
{
    /// <summary>
    /// Сервис создания тестовых данных <see cref="Fixture"/>
    /// </summary>
    private readonly Fixture _fixture;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetByIdHandlerTests(EventFixture<GetBookingByIdHandler> fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive(EventFixture<GetBookingByIdHandler> fixture) : GetByIdHandlerTests(fixture)
    {
        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();
            var booking = new Booking(@event.Id, Guid.NewGuid());
            var query = new GetBookingByIdQuery(booking.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                await context.Events.AddAsync(@event, TestCancellationToken);
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                // #### Act ####
                var handler = scope.ServiceProvider.GetRequiredService<GetBookingByIdHandler>();
                var actualResult = await handler.Handle(query, TestCancellationToken);

                // #### Assert ####
                actualResult.Should().NotBeNull();
                actualResult.Status.Should().Be(BookingStatus.Pending);
            }
        }

        /// <summary>
        /// Получение бронирования по идентификатору с последовательным изменением статуса и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_ChangeStatus_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();
            var booking = new Booking(@event.Id, Guid.NewGuid());
            var query = new GetBookingByIdQuery(booking.Id);

            BookingResponse actualResultPendingStatus;
            BookingResponse actualResultConfirmedStatus;

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                await context.Events.AddAsync(@event, TestCancellationToken);
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<GetBookingByIdHandler>();
                actualResultPendingStatus = await handler.Handle(query, TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualBooking = await context.Bookings.FindAsync([booking.Id], TestCancellationToken);

                actualBooking!.Confirm();

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<GetBookingByIdHandler>();
                actualResultConfirmedStatus = await handler.Handle(query, TestCancellationToken);
            }

            // #### Assert ####
            actualResultPendingStatus.Should().NotBeNull();
            actualResultPendingStatus.Status.Should().Be(BookingStatus.Pending);

            actualResultConfirmedStatus.Should().NotBeNull();
            actualResultConfirmedStatus.Status.Should().Be(BookingStatus.Confirmed);
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative(EventFixture<GetBookingByIdHandler> fixture) : GetByIdHandlerTests(fixture)
    {
        /// <summary>
        /// Получение отсутствующего бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingBooking_ThrowNotFoundException()
        {
            // #### Arrange ####
            var bookingId = Guid.NewGuid();
            var query = new GetBookingByIdQuery(bookingId);

            // #### Act ####
            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<GetBookingByIdHandler>();

                await handler.Handle(query, TestCancellationToken);
            };

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}