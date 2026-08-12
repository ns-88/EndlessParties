using AutoFixture;
using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Events.Services;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Contracts.Models;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Tests.Application.Services;

/// <summary>
/// Тесты для сервиса <see cref="EventService"/>
/// </summary>
public class EventServiceTests
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
    /// Контейнер <see cref="AutoMocker"/>
    /// </summary>
    private readonly AutoMocker _autoMocker;

    /// <summary>
    /// Сервис создания тестовых данных <see cref="Fixture"/>
    /// </summary>
    private readonly Fixture _fixture;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventServiceTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : EventServiceTests
    {
        /// <summary>
        /// Создание события с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var eventService = _autoMocker.CreateInstance<EventService>();

            var eventRequest = _fixture
                .Build<CreateEventRequest>()
                .With(x => x.TotalSeats, TotalSeats)
                .With(x => x.StartAt, StartAt)
                .With(x => x.EndAt, EndAt)
                .Create();

            var eventResponse = new EventResponse
            {
                Id = Guid.NewGuid(),
                Title = eventRequest.Title,
                TotalSeats = eventRequest.TotalSeats,
                AvailableSeats = eventRequest.TotalSeats,
                Description = eventRequest.Description,
                StartAt = eventRequest.StartAt,
                EndAt = eventRequest.EndAt
            };

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Create(It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var actualResult = await eventService.Create(eventRequest, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventResponse, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Create(It.IsAny<Event>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение всех событий для корректного фильтра с возвратом данных пагинации
        /// </summary>
        [Fact]
        public async Task GetAll_CorrectFilter_ReturnsValidEventPaginatedResponse()
        {
            // #### Arrange ####
            var eventService = _autoMocker.CreateInstance<EventService>();

            var queryFilter = _fixture
                .Build<GetAllEventsQueryFilter>()
                .With(x => x.Page, 1)
                .With(x => x.PageSize, 10)
                .Create();

            var eventItems = _fixture
                .Build<Event>()
                .FromFactory((string title, string ? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .CreateMany(1)
                .ToList();

            var collectionResult = new CollectionResult<Event>(20, eventItems);

            var eventPaginatedResponse = new EventPaginatedResponse
            {
                PageNumber = queryFilter.Page!.Value,
                PageSize = queryFilter.PageSize!.Value,
                TotalCount = 20,
                TotalPages = 2,
                Items =
                [
                    new EventResponse
                    {
                        Id = eventItems[0].Id,
                        Title = eventItems[0].Title,
                        TotalSeats = eventItems[0].TotalSeats,
                        AvailableSeats = eventItems[0].AvailableSeats,
                        Description = eventItems[0].Description,
                        StartAt = eventItems[0].StartAt,
                        EndAt = eventItems[0].EndAt
                    }
                ]
            };

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetAll(It.IsAny<GetAllEventsFilter>(), CancellationToken.None))
                .ReturnsAsync(collectionResult);

            // #### Act ####
            var actualResult = await eventService.GetAll(queryFilter, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventPaginatedResponse, x => x.Excluding(ctx => ctx.Path.EndsWith("Id")));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetAll(It.IsAny<GetAllEventsFilter>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение события по идентификатору для корректного фильтра с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectFilter_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var eventService = _autoMocker.CreateInstance<EventService>();
            var eventId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var eventResponse = new EventResponse
            {
                Id = Guid.NewGuid(),
                Title = @event.Title,
                TotalSeats = @event.TotalSeats,
                AvailableSeats = @event.TotalSeats,
                Description = @event.Description,
                StartAt = @event.StartAt,
                EndAt = @event.EndAt
            };

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(eventId, CancellationToken.None))
                .ReturnsAsync(@event);

            // #### Act ####
            var actualResult = await eventService.GetById(eventId, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventResponse, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Обновление события с корректными данными без генерации исключения
        /// </summary>
        [Fact]
        public async Task Update_CorrectData_NoThrowException()
        {
            // #### Arrange ####
            var eventService = _autoMocker.CreateInstance<EventService>();
            var eventId = Guid.NewGuid();

            var eventRequest = _fixture
                .Build<CreateEventRequest>()
                .With(x => x.TotalSeats, TotalSeats)
                .With(x => x.StartAt, StartAt)
                .With(x => x.EndAt, EndAt)
                .Create();

            var @event = new Event(eventRequest.Title, eventRequest.TotalSeats, eventRequest.Description, eventRequest.StartAt, eventRequest.EndAt);
            Event? actualEvent = null;

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None))
                .Callback((Guid _, Event eventArg, CancellationToken _) => actualEvent = eventArg)
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = () => eventService.Update(eventId, eventRequest, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();
            actualEvent.Should().BeEquivalentTo(@event, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Удаления события с корректными данными без генерации исключения
        /// </summary>
        [Fact]
        public async Task Remove_CorrectData_NoThrowException()
        {
            // #### Arrange ####
            var eventService = _autoMocker.CreateInstance<EventService>();
            var eventId = Guid.NewGuid();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Remove(eventId, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = () => eventService.Remove(eventId, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Remove(eventId, CancellationToken.None), Times.Once());

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}