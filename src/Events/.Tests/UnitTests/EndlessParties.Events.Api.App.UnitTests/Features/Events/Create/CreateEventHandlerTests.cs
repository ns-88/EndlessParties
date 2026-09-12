using AutoFixture;
using EndlessParties.Events.Api.App.Features.Events.Create;
using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Events.Create;

/// <summary>
/// Тесты для обработчика <see cref="CreateEventHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class CreateEventHandlerTests
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
    public CreateEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : CreateEventHandlerTests
    {
        /// <summary>
        /// Создание события с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateEventHandler>();

            var eventRequest = _fixture
                .Build<CreateEventRequest>()
                .With(x => x.TotalSeats, TestConstants.TotalSeats)
                .With(x => x.StartAt, TestConstants.StartAt)
                .With(x => x.EndAt, TestConstants.EndAt)
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

            var command = new CreateEventCommand(eventRequest);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Create(It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IUnitOfWork>()
                .Setup(x => x.SaveChangesAsync(CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var actualResult = await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventResponse, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Create(It.IsAny<Event>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUnitOfWork>()
                .Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}