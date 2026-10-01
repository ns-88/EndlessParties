using EndlessParties.Events.Api.App.Features.Bookings.GetById;
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

namespace EndlessParties.Events.Api.App.UnitTests.Features.Bookings.GetById;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="GetBookingByIdHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class GetBookingByIdHandlerTests
{
    /// <summary>
    /// Контейнер <see cref="AutoMocker"/>
    /// </summary>
    private readonly AutoMocker _autoMocker;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetBookingByIdHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict).Use<IUnitOfWork>(new FakeUnitOfWork());
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : GetBookingByIdHandlerTests
    {
        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetBookingByIdHandler>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());
            var query = new GetBookingByIdQuery(bookingId);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            // #### Act ####
            var actualResult = await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().NotBeNull();
            actualResult.Status.Should().Be(BookingStatus.Pending);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(bookingId, CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение бронирования по идентификатору с последовательным изменением статуса и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_ChangeStatus_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetBookingByIdHandler>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());
            var query = new GetBookingByIdQuery(bookingId);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            // #### Act ####
            var actualResultPendingStatus = await handler.Handle(query, CancellationToken.None);

            booking.Confirm();

            var actualResultConfirmedStatus = await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            actualResultPendingStatus.Should().NotBeNull();
            actualResultPendingStatus.Status.Should().Be(BookingStatus.Pending);

            actualResultConfirmedStatus.Should().NotBeNull();
            actualResultConfirmedStatus.Status.Should().Be(BookingStatus.Confirmed);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Exactly(2));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(bookingId, CancellationToken.None), Times.Exactly(2));

            _autoMocker.VerifyNoOtherCalls();
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative : GetBookingByIdHandlerTests
    {
        /// <summary>
        /// Получение отсутствующего бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingBooking_ThrowNotFoundException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetBookingByIdHandler>();
            var bookingId = Guid.NewGuid();
            var query = new GetBookingByIdQuery(bookingId);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(AdminUserContext);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new NotFoundException(string.Empty));

            // #### Act ####
            var action = async () => await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение бронирования созданного другим пользователем для пользователя с ролью "User"
        /// </summary>
        [Fact]
        public async Task GetById_WhenCreateBookingFromAnotherUser_ThrowForbiddenException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetBookingByIdHandler>();
            var bookingId = Guid.NewGuid();
            var query = new GetBookingByIdQuery(bookingId);
            var booking = new Booking(Guid.NewGuid(), Guid.NewGuid());

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .SetupGet(x => x.Current)
                .Returns(OrdinaryUserContext);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            // #### Act ####
            var action = async () => await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            await action.Should()
                .ThrowAsync<ForbiddenException>()
                .WithMessage(ApplicationErrors.Bookings.NotPossibleReceivingBookingFromAnotherUser);

            _autoMocker
                .GetMock<IUserContextAccessor>()
                .VerifyGet(x => x.Current, Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(bookingId, CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}