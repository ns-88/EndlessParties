using EndlessParties.Events.Api.App.Features.Bookings.GetById;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.UnitTests.Application.Fakes;
using EndlessParties.UnitTests.Application.Infrastructure;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace EndlessParties.Events.Api.App.UnitTests.Features.Bookings.GetById;

/// <summary>
/// Тесты для обработчика <see cref="GetByIdHandler"/>
/// </summary>
[Trait("Category", "Unit")]
public class GetByIdHandlerTests
{
    /// <summary>
    /// Контейнер <see cref="AutoMocker"/>
    /// </summary>
    private readonly AutoMocker _autoMocker;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetByIdHandlerTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict).Use<IUnitOfWork>(new FakeUnitOfWork());
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : GetByIdHandlerTests
    {
        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetByIdHandler>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid());
            var query = new GetByIdQuery(bookingId);

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
            var handler = _autoMocker.CreateInstance<GetByIdHandler>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid());
            var query = new GetByIdQuery(bookingId);

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
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(bookingId, CancellationToken.None), Times.Exactly(2));

            _autoMocker.VerifyNoOtherCalls();
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative : GetByIdHandlerTests
    {
        /// <summary>
        /// Получение отсутствующего бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingBooking_ThrowNotFoundException()
        {
            // #### Arrange ####
            var handler = _autoMocker.CreateInstance<GetByIdHandler>();
            var bookingId = Guid.NewGuid();
            var query = new GetByIdQuery(bookingId);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new NotFoundException(string.Empty));

            // #### Act ####
            var action = async () => await handler.Handle(query, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}