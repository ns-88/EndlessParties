using EndlessParties.Bookings.Api.Infrastructure.IntegrationTests.Fixtures;
using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Domain.Models;
using EndlessParties.Bookings.Repositories;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.Bookings.Api.Infrastructure.IntegrationTests.Repositories;

/// <summary>
/// Тесты для репозитория <see cref="BookingRepository"/>
/// </summary>
[Trait("Category", "Integration")]
public class BookingRepositoryTests : DatabaseIntegrationTest<BookingsDbContext, BookingFixture>
{
    /// <inheritdoc />
    public BookingRepositoryTests(BookingFixture fixture) : base(fixture)
    {
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : BookingRepositoryTests
    {
        /// <inheritdoc />
        public Positive(BookingFixture fixture) : base(fixture)
        {
        }


        /// <summary>
        /// Создание бронирования для корректных данных
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_AddNewBooking()
        {
            // #### Arrange ####
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await bookingRepository.Create(booking, TestCancellationToken);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            // #### Assert ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
                var actualResult = await context.Bookings.FindAsync([booking.Id], TestCancellationToken);

                actualResult.Should().BeEquivalentTo(booking, options => options
                    .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                    .WhenTypeIs<DateTimeOffset>());
            }
        }

        /// <summary>
        /// Получение бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_ExistingEvent_ReturnsValidEvent()
        {
            // #### Arrange ####
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BookingsDbContext>();
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();

                // #### Act ####
                var actualResult = await bookingRepository.GetById(booking.Id, TestCancellationToken);

                // #### Assert ####
                actualResult.Should().BeEquivalentTo(booking, options => options
                    .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                    .WhenTypeIs<DateTimeOffset>());
            }
        }
    }
}