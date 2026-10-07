using System.Collections.Concurrent;
using AutoFixture;
using EndlessParties.Bookings.Api.App.Features.Create;
using EndlessParties.Bookings.Api.App.UnitTests.Fakes;
using EndlessParties.Bookings.Api.App.UnitTests.Infrastructure;
using EndlessParties.Bookings.Domain.Enums;
using EndlessParties.Bookings.Domain.Errors;
using EndlessParties.Bookings.Domain.Models;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Bookings;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Models;
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
    /// Конструктор
    /// </summary>
    public CreateBookingHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict).Use<IUnitOfWork>(new FakeUnitOfWork());
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

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(BookingsActiveCount);

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

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker
                .GetMock<IEventBus>()
                .Verify(x => x.Publish(It.IsAny<BookingCreatedEvent>(), CancellationToken.None), Times.Once);

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
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(() => currentUserContextIdx++ == 0 ? blockedUserContext : activeUserContext);

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

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetActiveCountByUserId(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(Booking.MaxActiveCount + 1);

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
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetActiveCountByUserId(userContext.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}