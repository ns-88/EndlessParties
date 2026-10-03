using System.Collections.Concurrent;
using AutoFixture;
using EndlessParties.Events.Api.App.Features.Bookings.Create;
using EndlessParties.Events.Api.App.UnitTests.Infrastructure;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Models;
using EndlessParties.UnitTests.Application.Fakes;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Bookings.Api.App.UnitTests.Features.Create;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="CreateBookingHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class CreateBookingHandlerTests
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
    public CreateBookingHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict).Use<IUnitOfWork>(new FakeUnitOfWork());
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : CreateBookingHandlerTests
    {
        /// <summary>
        /// Создание бронирования с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var command = new CreateBookingCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IEventBus>()
                .Setup(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var actualResult = await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().NotBeNull();
            actualResult.Status.Should().Be(BookingStatus.Pending);
            @event.AvailableSeats.Should().Be(TotalSeats - 1);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var command = new CreateBookingCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IEventBus>()
                .Setup(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var bookingResponses = new List<BookingResponse>();

            for (var i = 0; i < TotalSeats; i++)
            {
                var actualResult = await handler.Handle(command, CancellationToken.None);
                bookingResponses.Add(actualResult);
            }

            // #### Assert ####
            bookingResponses.Should().OnlyHaveUniqueItems(x => x.Id);
            @event.AvailableSeats.Should().Be(0);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание нескольких бронирований в паралелльной среде для одного события и получение ожидаемого ответа
        /// </summary>
        [Theory]
        [InlineData(20, 5)]
        [InlineData(10, 10)]
        public async Task Create_ConcurrentMultipleBookings_ReturnsExpectedBookingResponse(int requestCount, int totalSeats)
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var command = new CreateBookingCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, totalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IEventBus>()
                .Setup(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            var bookingResponses = new ConcurrentBag<BookingResponse>();

            var tasks = Enumerable
                .Range(0, requestCount)
                .Select(_ => Task.Run(async () =>
                {
                    var actualResult = await handler.Handle(command, CancellationToken.None);
                    bookingResponses.Add(actualResult);
                }))
                .ToList();

            var results = await Task
                .WhenAll(tasks)
                .ContinueWith(x => x.Exception != null ? x.Exception.Flatten().InnerExceptions : []);

            // #### Assert ####
            results.Should().HaveCount(requestCount - totalSeats)
                .And.AllBeOfType<ConflictException>();

            bookingResponses.Should().HaveCount(totalSeats)
                .And.OnlyHaveUniqueItems(x => x.Id);

            @event.AvailableSeats.Should().Be(0);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Exactly(requestCount));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(totalSeats));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None), Times.Exactly(requestCount));

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Exactly(requestCount));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Exactly(requestCount));

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Exactly(totalSeats));

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// При создании бронирований, ограничения одного пользователя не влияют на ограничения другого
        /// </summary>
        [Fact]
        public async Task Create_ShouldIsolateLimits_AllowingNewBookingEvenIfAnotherUserIsBlocked()
        {
            // #### Arrange ####
            var eventId = Guid.NewGuid();
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            var command = new CreateBookingCommand(eventId);
            var currentUserContextIdx = 0;
            var blockedUserContext = new UserContext
            {
                Id = Guid.NewGuid(),
                Name = "BlockedUser",
                Role = UserRole.User
            };
            var activeUserContext = new UserContext
            {
                Id = Guid.NewGuid(),
                Name = "ActiveUser",
                Role = UserRole.User
            };

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(() => currentUserContextIdx++ == 0 ? blockedUserContext : activeUserContext);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(blockedUserContext.Id, CancellationToken.None))
                .ReturnsAsync(Booking.MaxActiveCount + 1);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(activeUserContext.Id, CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

            _autoMocker
                .GetMock<IEventBus>()
                .Setup(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act/Assert ####
            var action = async () => await handler.Handle(command, CancellationToken.None);
            await action.Should().ThrowAsync<ConflictException>();

            var actualResult = await handler.Handle(command, CancellationToken.None);
            actualResult.Should().NotBeNull();
            actualResult.Status.Should().Be(BookingStatus.Pending);
            @event.AvailableSeats.Should().Be(TotalSeats - 1);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Exactly(2));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(blockedUserContext.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(activeUserContext.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Exactly(2));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Exactly(2));

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative : CreateBookingHandlerTests
    {
        /// <summary>
        /// Создание бронирования для отсутствующего события
        /// </summary>
        [Fact]
        public async Task Create_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var command = new CreateBookingCommand(eventId);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new NotFoundException(string.Empty));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого исключения в случае превышения количества доступных мест
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_GreaterThanAvailableSeats_ThrowConflictException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var userContext = AdminUserContext;
            var command = new CreateBookingCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(userContext);

            _autoMocker
                .GetMock<IEventBus>()
                .Setup(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // #### Act ####
            for (var i = 0; i < TotalSeats; i++)
            {
                await handler.Handle(command, CancellationToken.None);
            }

            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<ConflictException>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Exactly(TotalSeats + 1));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(userContext.Id, CancellationToken.None), Times.Exactly(TotalSeats + 1));

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Exactly(TotalSeats + 1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Exactly(TotalSeats + 1));

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Exactly(TotalSeats));

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание бронирования для события, которое уже началось
        /// </summary>
        [Fact]
        public async Task Create_WhenEventAlreadyStarted_ThrowLogicException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var userContext = AdminUserContext;
            var command = new CreateBookingCommand(eventId);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(userContext);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should()
                .ThrowAsync<LogicException>()
                .WithInnerException(typeof(LogicException))
                .WithMessage(ApplicationErrors.Bookings.EventAlreadyStarted);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание новой брони пользователем, у которого превышено максимальное количество бронирований
        /// </summary>
        [Fact]
        public async Task Create_WhenUserAvailableSeatsExceeded_ThrowConflictException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CreateBookingHandler>();
            var eventId = Guid.NewGuid();
            var userContext = AdminUserContext;
            var command = new CreateBookingCommand(eventId);
            var expectedMessage = string.Format(ApplicationErrors.Bookings.AvailableSeatsExceeded, Booking.MaxActiveCount);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(Booking.MaxActiveCount + 1);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Setup(x => x.UtcNow())
                .Returns(StartAt.AddDays(-1));

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(userContext);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should()
                .ThrowAsync<ConflictException>()
                .WithMessage(expectedMessage);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(userContext.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IDateTimeProvider>()
                .Verify(x => x.UtcNow(), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}