using AutoFixture;
using EndlessParties.Database.Database;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Bookings.Repositories;
using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Xunit;

namespace EndlessParties.IntegrationTests.Infrastructure.Repositories;

/// <summary>
/// Тесты для репозитория <see cref="BookingRepository"/>
/// </summary>
public class BookingRepositoryTests : BaseIntegrationTest<EventsDbContext>
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
    public BookingRepositoryTests(PostgreSqlContainerFixture<EventsDbContext> fixture)
        : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : BookingRepositoryTests
    {
        /// <inheritdoc />
        public Positive(PostgreSqlContainerFixture<EventsDbContext> fixture)
            : base(fixture)
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

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                context.Events.Add(@event);
                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var bookingRepository = new BookingRepository(context);
                var unitOfWork = new DefaultUnitOfWork<EventsDbContext>(context);

                await bookingRepository.Create(booking, TestCancellationToken);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            // #### Assert ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
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

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                await context.Events.AddAsync(@event, TestCancellationToken);
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var bookingRepository = new BookingRepository(context);

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