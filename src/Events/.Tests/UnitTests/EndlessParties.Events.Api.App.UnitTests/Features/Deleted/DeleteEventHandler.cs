using EndlessParties.Events.Api.App.Features.Deleted;
using EndlessParties.Events.Domain.Events;
using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Utils.UnitTests;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Deleted;

/// <summary>
/// Тесты для обработчика <see cref="DeletedEventHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class DeletedEventHandlerTests
{
    /// <summary>
    /// Контейнер <see cref="AutoMocker"/>
    /// </summary>
    private readonly AutoMocker _autoMocker;


    /// <summary>
    /// Конструктор
    /// </summary>
    public DeletedEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : DeletedEventHandlerTests
    {
        /// <summary>
        /// Вызов обработчика доменного события при удалении мероприятия (события)
        /// </summary>
        [Fact]
        public async Task Handle_WhenDeleted_ReturnsExpectedResult()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<DeletedEventHandler>();
            var @event = new EventDeletedDomainEvent(Guid.NewGuid());

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Remove(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var action = async () => await handler.Handle(@event, CancellationToken.None);

            // #### Assert ####
            await action.Should().NotThrowAsync();

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Remove(It.IsAny<string>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}