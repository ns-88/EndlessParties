using AutoFixture;
using EndlessParties.Database.Database;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Infrastructure.Bookings.Repositories;
using EndlessParties.IntegrationTests.Infrastructure.Fixtures;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.IntegrationTests.Infrastructure.Repositories;

/// <summary>
/// Тесты для репозитория <see cref="BookingRepository"/>
/// </summary>
[Trait("Category", "Integration")]
public class BookingRepositoryTests : BaseIntegrationTest<EventsDbContext, EventFixture>
{
    /// <summary>
    /// Дата и время начала события
    /// </summary>
    private static readonly DateTimeOffset StartAt = new(new DateTime(2025, 01, 01), TimeSpan.Zero);

    /// <summary>
    /// Дата и время завершения события
    /// </summary>
    private static readonly DateTimeOffset EndAt = new(new DateTime(2025, 01, 02), TimeSpan.Zero);

    /// <summary>
    /// Общее количество мест
    /// </summary>
    private const int TotalSeats = 3;

    /// <summary>
    /// Сервис создания тестовых данных
    /// </summary>
    private readonly Fixture _fixture;


    /// <inheritdoc />
    public BookingRepositoryTests(EventFixture fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : BookingRepositoryTests
    {
        /// <inheritdoc />
        public Positive(EventFixture fixture) : base(fixture)
        {
        }


        /// <summary>
        /// Создание бронирования для корректных данных
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_AddNewBooking()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var booking = new Booking(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

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
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
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
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var booking = new Booking(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();

                await context.Events.AddAsync(@event, TestCancellationToken);
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
                    .Excluding(e => e.Event)
                    .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1)))
                    .WhenTypeIs<DateTimeOffset>());
            }
        }
    }
}