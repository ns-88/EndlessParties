using AutoFixture;
using EndlessParties.Events.Api.App.Features.Bookings.Cancel;
using EndlessParties.Events.Api.App.UnitTests.Features.Bookings.Create;
using EndlessParties.Events.Api.App.UnitTests.Infrastructure;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.UnitTests.Application.Fakes;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Bookings.Cancel;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="CancelBookingHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class CancelBookingHandlerTests
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
    public CancelBookingHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict).Use<IUnitOfWork>(new FakeUnitOfWork());
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : CancelBookingHandlerTests
    {
        /// <summary>
        /// Отмена бронирования с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Cancel_CorrectData_ReturnsValidResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CancelBookingHandler>();
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId, AdminUserId);
            var command = new CancelBookingCommand(booking.Id);

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            @event.TryReserveSeats();
            booking.Confirm();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetByIdWithLock(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            // #### Act ####
            await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            booking.Status.Should().Be(BookingStatus.Canceled);
            @event.AvailableSeats.Should().Be(TotalSeats);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetByIdWithLock(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(booking.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .Verify(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative : CancelBookingHandlerTests
    {
        /// <summary>
        /// Отмена бронирования созданного другим пользователем для пользователя с ролью "User"
        /// </summary>
        [Fact]
        public async Task Cancel_WhenCreateBookingFromAnotherUser_ThrowForbiddenException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<CancelBookingHandler>();
            var eventId = Guid.NewGuid();
            var booking = new Booking(eventId, Guid.NewGuid());
            var command = new CancelBookingCommand(booking.Id);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(OrdinaryUserContext);

            // #### Act ####
            var action = async () => await handler.Handle(command, CancellationToken.None);

            // #### Assert ####
            await action.Should()
                .ThrowAsync<ForbiddenException>()
                .WithMessage(ApplicationErrors.Bookings.NotPossibleCancelBookingFromAnotherUser);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(booking.Id, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .Verify(x => x.Current, Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}