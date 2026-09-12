using EndlessParties.Events.Api.App.Features.Events.Remove;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Events.Remove;

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
    /// Конструктор
    /// </summary>
    public DeleteEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : DeleteEventHandlerTests
    {
        /// <summary>
        /// Удаления события с корректными данными без генерации исключения
        /// </summary>
        [Fact]
        public async Task Remove_CorrectData_NoThrowException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<DeleteEventHandler>();
            var eventId = Guid.NewGuid();
            var command = new DeleteEventCommand(eventId);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Remove(eventId, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Remove(eventId, CancellationToken.None), Times.Once());

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}