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
    private static readonly DateTime StartAt = new(2025, 01, 01);

    /// <summary>
    /// Дата и время завершения события
    /// </summary>
    private static readonly DateTime EndAt = new(2025, 01, 02);

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
    /// Создание события с корректными данными и получение ответа с данными события
    /// </summary>
    [Fact]
    public async Task Create_CorrectData_ReturnsValidEventResponse()
    {
        // #### Arrange ####
        var eventService = _autoMocker.CreateInstance<EventService>();

        var eventRequest = _fixture
            .Build<CreateEventRequest>()
            .With(x => x.StartAt, StartAt)
            .With(x => x.EndAt, EndAt)
            .Create();

        var eventResponse = new EventResponse
        {
            Id = Guid.NewGuid(),
            Title = eventRequest.Title,
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

        _autoMocker
            .GetMock<IEventRepository>()
            .VerifyNoOtherCalls();
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
            .FromFactory((string title, string? description) => new Event(title, description, StartAt, EndAt))
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

        _autoMocker
            .GetMock<IEventRepository>()
            .VerifyNoOtherCalls();
    }

    /// <summary>
    /// Получение события по идентификатору для корректного фильтра с возвратом ответа с данными события
    /// </summary>
    [Fact]
    public async Task GetById_CorrectFilter_ReturnsValidEventResponse()
    {
        // #### Arrange ####
        var eventService = _autoMocker.CreateInstance<EventService>();
        var id = Guid.NewGuid();

        var @event = _fixture
            .Build<Event>()
            .FromFactory((string title, string? description) => new Event(title, description, StartAt, EndAt))
            .Create();

        var eventResponse = new EventResponse
        {
            Id = Guid.NewGuid(),
            Title = @event.Title,
            Description = @event.Description,
            StartAt = @event.StartAt,
            EndAt = @event.EndAt
        };

        _autoMocker
            .GetMock<IEventRepository>()
            .Setup(x => x.GetById(id, CancellationToken.None))
            .ReturnsAsync(@event);

        // #### Act ####
        var actualResult = await eventService.GetById(id, CancellationToken.None);

        // #### Assert ####
        actualResult.Should().BeEquivalentTo(eventResponse, x => x.Excluding(e => e.Id));

        _autoMocker
            .GetMock<IEventRepository>()
            .Verify(x => x.GetById(id, CancellationToken.None), Times.Once);

        _autoMocker
            .GetMock<IEventRepository>()
            .VerifyNoOtherCalls();
    }

    /// <summary>
    /// Обновление события с корректными данными без генерации исключения
    /// </summary>
    [Fact]
    public async Task Update_CorrectData_NoThrowException()
    {
        // #### Arrange ####
        var eventService = _autoMocker.CreateInstance<EventService>();
        var id = Guid.NewGuid();

        var eventRequest = _fixture
            .Build<CreateEventRequest>()
            .With(x => x.StartAt, StartAt)
            .With(x => x.EndAt, EndAt)
            .Create();

        var @event = new Event(eventRequest.Title, eventRequest.Description, eventRequest.StartAt, eventRequest.EndAt);
        Event? actualEvent = null;

        _autoMocker
            .GetMock<IEventRepository>()
            .Setup(x => x.Update(id, It.IsAny<Event>(), CancellationToken.None))
            .Callback((Guid _, Event eventArg, CancellationToken _) => actualEvent = eventArg)
            .Returns(Task.CompletedTask);

        // #### Act ####
        var action = () => eventService.Update(id, eventRequest, CancellationToken.None);

        // #### Assert ####
        await action.Should().NotThrowAsync();
        actualEvent.Should().BeEquivalentTo(@event, x => x.Excluding(e => e.Id));

        _autoMocker
            .GetMock<IEventRepository>()
            .Verify(x => x.Update(id, It.IsAny<Event>(), CancellationToken.None), Times.Once);

        _autoMocker
            .GetMock<IEventRepository>()
            .VerifyNoOtherCalls();
    }

    /// <summary>
    /// Удаления события с корректными данными без генерации исключения
    /// </summary>
    [Fact]
    public async Task Remove_CorrectData_NoThrowException()
    {
        // #### Arrange ####
        var eventService = _autoMocker.CreateInstance<EventService>();
        var id = Guid.NewGuid();

        _autoMocker
            .GetMock<IEventRepository>()
            .Setup(x => x.Remove(id, CancellationToken.None))
            .Returns(Task.CompletedTask);

        // #### Act ####
        var action = () => eventService.Remove(id, CancellationToken.None);

        // #### Assert ####
        await action.Should().NotThrowAsync();

        _autoMocker
            .GetMock<IEventRepository>()
            .Verify(x => x.Remove(id, CancellationToken.None), Times.Once());

        _autoMocker
            .GetMock<IEventRepository>()
            .VerifyNoOtherCalls();
    }
}