using EndlessParties.Bookings.Api.App.Features.Create;
using EndlessParties.Bookings.Api.App.Features.GetById;
using EndlessParties.Bookings.Api.App.IntegrationTests.Fixtures;
using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Domain.Enums;
using EndlessParties.Bookings.Domain.Models;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.Bookings.Api.App.IntegrationTests.Features.GetById;

/// <summary>
/// Тесты для обработчика <see cref="GetBookingByIdHandler"/>
/// </summary>
[Trait("Category", "Integration")]
public class GetByIdHandlerTests : DatabaseIntegrationTest<BookingsDbContext, BookingFixture<GetBookingByIdHandler>>
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public GetByIdHandlerTests(BookingFixture<GetBookingByIdHandler> fixture) : base(fixture)
    {
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive(BookingFixture<GetBookingByIdHandler> fixture) : GetByIdHandlerTests(fixture)
    {
        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());
            var query = new GetBookingByIdQuery(booking.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
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
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());
            var query = new GetBookingByIdQuery(booking.Id);

            BookingResponse actualResultPendingStatus;
            BookingResponse actualResultConfirmedStatus;

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
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
                var context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
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
    public class Negative(BookingFixture<GetBookingByIdHandler> fixture) : GetByIdHandlerTests(fixture)
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