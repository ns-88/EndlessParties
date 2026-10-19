using AutoFixture;
using EndlessParties.Events.Api.Infrastructure.UnitTests.Infrastructure;
using EndlessParties.Events.Data;
using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Shared.Cache.Abstractions;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.Infrastructure.UnitTests.Data;

/// <summary>
/// Тесты для обработчика <see cref="CachingEventDataProvider"/>
/// </summary>
[Trait("Category", "Unit")]
public class CachingEventDataProviderTests
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
    public CachingEventDataProviderTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : CachingEventDataProviderTests
    {
        /// <summary>
        /// Получение события по идентификатору с попаданием в кэш и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CacheHit_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var dataProvider = _autoMocker.CreateInstance<CachingEventDataProvider>();
            var eventId = Guid.NewGuid();
            var eventModel = _fixture.Create<EventModel>();

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Get<EventModel>(It.IsAny<string>()))
                .ReturnsAsync(eventModel);

            // #### Act ####
            var actualResult = await dataProvider.GetById(eventId, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventModel);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Never);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Get<EventModel>(It.IsAny<string>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение события по идентификатору с кэш-промахом и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CacheMiss_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var dataProvider = _autoMocker.CreateInstance<CachingEventDataProvider>();
            var eventId = Guid.NewGuid();
            var eventModel = _fixture.Create<EventModel>();

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Get<EventModel>(It.IsAny<string>()))
                .ReturnsAsync((EventModel?)null);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Setup(x => x.GetById(eventId, CancellationToken.None))
                .ReturnsAsync(eventModel);

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<EventModel>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var actualResult = await dataProvider.GetById(eventId, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventModel);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Get<EventModel>(It.IsAny<string>()), Times.Once);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Set(It.IsAny<string>(), It.IsAny<EventModel>(), It.IsAny<TimeSpan>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение 10 самых популярных событий с попаданием в кэш и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetTop10MostFilled_CacheHit_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var dataProvider = _autoMocker.CreateInstance<CachingEventDataProvider>();
            var eventModelList = _fixture.CreateMany<EventTopModel>(3).ToList();

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Get<IReadOnlyList<EventTopModel>>(It.IsAny<string>()))
                .ReturnsAsync(eventModelList);

            // #### Act ####
            var actualResult = await dataProvider.GetTop10MostFilled(CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventModelList);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Verify(x => x.GetTop10MostFilled(CancellationToken.None), Times.Never);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Get<IReadOnlyList<EventTopModel>>(It.IsAny<string>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение 10 самых популярных событий с кэш-промахом и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetTop10MostFilled_CacheMiss_ReturnsValidEventResponse()
        {
            // #### Arrange ####
            var dataProvider = _autoMocker.CreateInstance<CachingEventDataProvider>();
            var eventModelList = _fixture.CreateMany<EventTopModel>(3).ToList();

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Get<IReadOnlyList<EventTopModel>>(It.IsAny<string>()))
                .ReturnsAsync((IReadOnlyList<EventTopModel>?)null);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Setup(x => x.GetTop10MostFilled(CancellationToken.None))
                .ReturnsAsync(eventModelList);

            _autoMocker
                .GetMock<ICacheClient>()
                .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<IReadOnlyList<EventTopModel>>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var actualResult = await dataProvider.GetTop10MostFilled(CancellationToken.None);

            // #### Assert ####
            actualResult.Should().BeEquivalentTo(eventModelList);

            _autoMocker
                .GetMock<IEventDataProvider>()
                .Verify(x => x.GetTop10MostFilled(CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Get<IReadOnlyList<EventTopModel>>(It.IsAny<string>()), Times.Once);

            _autoMocker
                .GetMock<ICacheClient>()
                .Verify(x => x.Set(It.IsAny<string>(), It.IsAny<IReadOnlyList<EventTopModel>>(), It.IsAny<TimeSpan>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}