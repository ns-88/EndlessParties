using AutoFixture;
using EndlessParties.Events.Api.App.Features.Events.Update;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Events.Update;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="UpdateEventHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class UpdateEventHandlerTests
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
    public UpdateEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : UpdateEventHandlerTests
    {
        /// <summary>
        /// Обновление события с корректными данными без генерации исключения
        /// </summary>
        [Fact]
        public async Task Update_CorrectData_NoThrowException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<UpdateEventHandler>();
            var eventId = Guid.NewGuid();

            var eventRequest = _fixture
                .Build<UpdateEventRequest>()
                .With(x => x.TotalSeats, TotalSeats)
                .With(x => x.StartAt, StartAt)
                .With(x => x.EndAt, EndAt)
                .Create();

            var @event = new Event(eventRequest.Title, eventRequest.TotalSeats, eventRequest.Description, eventRequest.StartAt, eventRequest.EndAt);
            var actualEvent = new Event("Title", TotalSeats, null, DateTimeOffset.Now, DateTimeOffset.Now);

            var command = new UpdateEventCommand(eventId, eventRequest);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(eventId, CancellationToken.None))
                .ReturnsAsync(actualEvent);

            _autoMocker
                .GetMock<IUnitOfWork>()
                .Setup(x => x.SaveChangesAsync(CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();
            actualEvent.Should().BeEquivalentTo(@event, x => x.Excluding(e => e.Id));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUnitOfWork>()
                .Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}