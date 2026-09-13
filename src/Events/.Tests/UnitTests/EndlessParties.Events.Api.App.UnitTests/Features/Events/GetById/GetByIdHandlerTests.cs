using AutoFixture;
using EndlessParties.Events.Api.App.Features.Events.GetById;
using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Events.GetById;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="GetByIdHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class GetByIdHandlerTests
{
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
    public GetByIdHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : GetByIdHandlerTests
    {
        /// <summary>
        /// Получение события по идентификатору для корректного фильтра с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectFilter_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetByIdHandler>();
            var eventId = Guid.NewGuid();
            var query = new GetByIdQuery(eventId);

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
            var actualResult = await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventResponse, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}