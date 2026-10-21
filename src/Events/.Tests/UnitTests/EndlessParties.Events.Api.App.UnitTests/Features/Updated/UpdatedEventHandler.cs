using EndlessParties.Events.Api.App.Features.Updated;
using EndlessParties.Events.Domain.Events;
using EndlessParties.Shared.Cache.Abstractions;
using EndlessParties.Shared.Utils.UnitTests;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Updated;

/// <summary>
/// Тесты для обработчика <see cref="UpdatedEventHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class UpdatedEventHandlerTests
{
    /// <summary>
    /// Контейнер <see cref="AutoMocker"/>
    /// </summary>
    private readonly AutoMocker _autoMocker;


    /// <summary>
    /// Конструктор
    /// </summary>
    public UpdatedEventHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : UpdatedEventHandlerTests
    {
        /// <summary>
        /// Вызов обработчика доменного события при обновлении мероприятия (события)
        /// </summary>
        [Fact]
        public async Task Handle_WhenUpdated_ReturnsExpectedResult()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<UpdatedEventHandler>();
            var @event = new EventUpdatedDomainEvent(Guid.NewGuid());

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