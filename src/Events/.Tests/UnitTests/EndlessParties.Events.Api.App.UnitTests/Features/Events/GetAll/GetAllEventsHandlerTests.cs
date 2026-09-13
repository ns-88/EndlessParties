using AutoFixture;
using EndlessParties.Events.Api.App.Features.Events.GetAll;
using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Events.Repositories.Abstractions.Models;
using EndlessParties.Shared.Contracts.Models;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Events.GetAll;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="GetAllEventsHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class GetAllEventsHandlerTests
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
    public GetAllEventsHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : GetAllEventsHandlerTests
    {
        /// <summary>
        /// Получение всех событий для корректного фильтра с возвратом данных пагинации
        /// </summary>
        [Fact]
        public async Task GetAll_CorrectFilter_ReturnsValidEventPaginatedResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetAllEventsHandler>();

            var queryFilter = _fixture
                .Build<EventSearchFilter>()
                .With(x => x.Page, 1)
                .With(x => x.PageSize, 10)
                .Create();

            var eventItems = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
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

            var query = new GetAllEventsQuery(queryFilter);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetAll(It.IsAny<GetAllEventsFilter>(), CancellationToken.None))
                .ReturnsAsync(collectionResult);

            // #### Act ####
            var actualResult = await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventPaginatedResponse, x => x.Excluding(ctx => ctx.Path.EndsWith("Id")));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetAll(It.IsAny<GetAllEventsFilter>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}