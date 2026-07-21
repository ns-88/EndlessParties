using AutoFixture;
using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Application.Abstractions.Bookings.Models.Requests;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Bookings.Services;
using EndlessParties.Domain.Enums;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;
using static EndlessParties.Domain.Errors.ApplicationErrors;

namespace EndlessParties.Tests.Application.Services;

/// <summary>
/// Тесты для сервиса <see cref="BookingService"/>
/// </summary>
public class BookingServiceTests
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
    public BookingServiceTests()
    {
        _autoMocker = new AutoMocker(MockBehavior.Strict);
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : BookingServiceTests
    {
        /// <summary>
        /// Создание бронирования с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var bookingRequest = _fixture.Create<CreateBookingRequest>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Exists(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(true);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            var actualResult = await bookingService.Create(bookingRequest, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().NotBeNull();
            actualResult.Status.Should().Be(BookingStatus.Pending);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Exists(bookingRequest.EventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Verify(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var bookingRequest = _fixture.Create<CreateBookingRequest>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Exists(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(true);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            var bookingResponses = new List<BookingResponse>();

            for (var i = 0; i < 3; i++)
            {
                var actualResult = await bookingService.Create(bookingRequest, CancellationToken.None);

                bookingResponses.Add(actualResult);
            }

            // #### Assert ####
            bookingResponses.Should().OnlyHaveUniqueItems(x => x.Id);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Exists(bookingRequest.EventId, CancellationToken.None), Times.Exactly(3));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(3));

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Verify(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()), Times.Exactly(3));

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid());

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            // #### Act ####
            var actualResult = await bookingService.GetById(bookingId, CancellationToken.None);

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
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var bookingId = Guid.NewGuid();
            var booking = new Booking(Guid.NewGuid());

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(booking);

            // #### Act ####
            var actualResultPendingStatus = await bookingService.GetById(bookingId, CancellationToken.None);

            booking.Confirm();

            var actualResultConfirmedStatus = await bookingService.GetById(bookingId, CancellationToken.None);

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
    public class Negative : BookingServiceTests
    {
        /// <summary>
        /// Создание бронирования для отсутствующего события
        /// </summary>
        [Fact]
        public async Task Create_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var createRequest = _fixture.Create<CreateBookingRequest>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Exists(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(false);

            // #### Act ####
            var action = () => bookingService.Create(createRequest, CancellationToken.None);

            // #### Assert ####
            (await action.Should().ThrowAsync<LogicException>()).WithInnerException<NotFoundException>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Exists(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Получение отсутствующего бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingBooking_ThrowNotFoundException()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var bookingId = Guid.NewGuid();

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new NotFoundException(string.Empty));

            // #### Act ####
            var action = () => bookingService.GetById(bookingId, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}