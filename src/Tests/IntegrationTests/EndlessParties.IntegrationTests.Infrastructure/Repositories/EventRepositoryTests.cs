using AutoFixture;
using EndlessParties.Database.Database;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Events.Repositories;
using EndlessParties.Shared.Utils.Database;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EndlessParties.IntegrationTests.Infrastructure.Repositories;

/// <summary>
/// Тесты для репозитория <see cref="EventRepository"/>
/// </summary>
public class EventRepositoryTests : BaseIntegrationTest<EventsDbContext>
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


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventRepositoryTests(PostgreSqlContainerFixture<EventsDbContext> fixture)
        : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : EventRepositoryTests
    {
        /// <inheritdoc />
        public Positive(PostgreSqlContainerFixture<EventsDbContext> fixture) : base(fixture)
        {
        }

        /// <summary>
        /// Создание события для корректных данных
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_AddNewEvent()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            // #### Act ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var eventRepository = new EventRepository(context);
                var unitOfWork = new DefaultUnitOfWork<EventsDbContext>(context);

                await eventRepository.Create(@event, TestCancellationToken);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            // #### Assert ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var actualResult = await context.Events.FindAsync([@event.Id], TestCancellationToken);

                actualResult.Should().BeEquivalentTo(@event);
            }
        }

        /// <summary>
        /// Получение события по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_ExistingEvent_ReturnsValidEvent()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                context.Events.Add(@event);
                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var eventRepository = new EventRepository(context);

                // #### Act ####
                var actualResult = await eventRepository.GetById(@event.Id, CancellationToken.None);

                // #### Assert ####
                actualResult.Should().BeEquivalentTo(@event);
            }
        }

        /// <summary>
        /// Обновление события по идентификатору с изменением значений существующих данных
        /// </summary>
        [Fact]
        public async Task Update_ExistingEvent_UpdatesFields()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var newEvent = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats + 5, description, StartAt.AddDays(1), EndAt.AddDays(1)))
                .Create();

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                context.Events.Add(@event);
                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var eventRepository = new EventRepository(context);
                var unitOfWork = new DefaultUnitOfWork<EventsDbContext>(context);
                var existingEvent = await eventRepository.GetById(@event.Id, TestCancellationToken);

                existingEvent.ChangeTitle(newEvent.Title);
                existingEvent.ChangeDescription(newEvent.Description);
                existingEvent.ChangeTotalSeats(newEvent.TotalSeats);
                existingEvent.ChangeStartAndEndAt(newEvent.StartAt, newEvent.EndAt);

                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            // #### Assert ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var actualResult = await context.Events.FindAsync([@event.Id], TestCancellationToken);

                actualResult.Should().BeEquivalentTo(newEvent, x => x.Excluding(e => e.Id).Excluding(e => e.RowVersion));
            }
        }

        /// <summary>
        /// Удаление события по идентификатору
        /// </summary>
        [Fact]
        public async Task Remove_ExistingEvent_RemovesElement()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) =>
                {
                    var localEvent = new Event(title, TotalSeats, description, StartAt, EndAt);
                    var booking = new Booking(localEvent.Id);
                    localEvent.Bookings.Add(booking);

                    return localEvent;
                })
                .Create();

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                context.Events.Add(@event);
                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var eventRepository = new EventRepository(context);
                var unitOfWork = new DefaultUnitOfWork<EventsDbContext>(context);

                await eventRepository.Remove(@event.Id, TestCancellationToken);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            // #### Assert ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                var deletedEvent = await context.Events.FindAsync([@event.Id], TestCancellationToken);

                deletedEvent.Should().BeNull();

                var actualBookings = await context.Bookings
                    .Where(x => x.EventId == @event.Id)
                    .ToListAsync(TestCancellationToken);

                actualBookings.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Получение всех событий для различных корректных фильтров с возвратом валидной коллекции с указанием общего числа элементов
        /// </summary>
        [Theory]
        [ClassData(typeof(EventFilterTestData))]
        public async Task GetAll_CorrectFilter_ReturnsValidCollectionResult(EventFilterTestCase testCase)
        {
            // #### Arrange ####
            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                foreach (var @event in testCase.Events)
                {
                    context.Events.Add(@event);
                }

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var context = await DbContextFactory.CreateDbContextAsync(TestCancellationToken))
            {
                // #### Act ####
                var eventRepository = new EventRepository(context);
                var actualResult = await eventRepository.GetAll(testCase.Filter, TestCancellationToken);

                // #### Assert ####
                actualResult.Should().BeEquivalentTo(testCase.CollectionResult, x => x.Excluding(ctx => ctx.Path.EndsWith("Id")).WithStrictOrdering());
            }
        }
    }
}