using AutoFixture;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Events.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Tests.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

namespace EndlessParties.Tests.Infrastructure.Repositories;

/// <summary>
/// Тесты для репозитория <see cref="EventRepository"/>
/// </summary>
public class EventRepositoryTests
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
    public EventRepositoryTests()
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : EventRepositoryTests
    {
        /// <summary>
        /// Создание события для корректных данных
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_AddNewEvent()
        {
            // #### Arrange ####
            var eventRepository = new EventRepository(null!);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            // #### Act ####
            await eventRepository.Create(@event, CancellationToken.None);

            var actualResult = await eventRepository.GetById(@event.Id, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(@event, x => x.Excluding(e => e.Id));
        }

        /// <summary>
        /// Получение всех событий для различных корректных фильтров с возвратом валидной коллекции с указанием общего числа элементов
        /// </summary>
        [Theory]
        [ClassData(typeof(EventFilterTestData))]
        public async Task GetAll_CorrectFilter_ReturnsValidCollectionResult(EventFilterTestCase testCase)
        {
            // #### Arrange ####
            var eventRepository = EventRepository.FromData(testCase.Events);

            // #### Act ####
            var actualResult = await eventRepository.GetAll(testCase.Filter, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(testCase.CollectionResult, x => x.Excluding(ctx => ctx.Path.EndsWith("Id")).WithStrictOrdering());
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

            var eventRepository = EventRepository.FromData([@event]);

            // #### Act ####
            var actualResult = await eventRepository.GetById(@event.Id, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(@event, x => x.Excluding(e => e.Id));
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
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var eventRepository = EventRepository.FromData([@event]);

            // #### Act ####
            //await eventRepository.Update(@event.Id, newEvent, CancellationToken.None);

            var actualResult = await eventRepository.GetById(@event.Id, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(newEvent, x => x.Excluding(e => e.Id));
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
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var eventRepository = EventRepository.FromData([@event]);

            // #### Act ####
            await eventRepository.Remove(@event.Id, CancellationToken.None);

            var action = () => eventRepository.GetById(@event.Id, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative : EventRepositoryTests
    {
        /// <summary>
        /// Получение отсутствующего события по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var nonExistingId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var eventRepository = EventRepository.FromData([@event]);

            // #### Act ####
            var action = () => eventRepository.GetById(nonExistingId, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Обновление отсутствующего события по идентификатору
        /// </summary>
        [Fact]
        public async Task Update_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var nonExistingId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var eventRepository = EventRepository.FromData([@event]);

            // #### Act ####
            //var action = () => eventRepository.Update(nonExistingId, @event, CancellationToken.None);

            // #### Assert ####
            //await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}