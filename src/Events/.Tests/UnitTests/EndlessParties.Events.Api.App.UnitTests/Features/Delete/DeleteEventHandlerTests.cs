using AutoFixture;
using EndlessParties.Events.Api.App.Features.Delete;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.UnitTests;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Delete;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="DeleteEventHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class DeleteEventHandlerTests
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
    public DeleteEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : DeleteEventHandlerTests
    {
        /// <summary>
        /// Удаление события с корректными данными без генерации исключения
        /// </summary>
        [Fact]
        public async Task Remove_CorrectData_NoThrowException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<DeleteEventHandler>();
            var eventId = Guid.NewGuid();
            var command = new DeleteEventCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(eventId, CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IUnitOfWork>()
                .Setup(x => x.SaveChangesAsync(CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();
            @event.IsDeleted.Should().BeTrue();

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