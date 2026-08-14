using System.Collections.Concurrent;
using AutoFixture;
using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
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

namespace EndlessParties.Tests.Application.Services;

/// <summary>
/// Тесты для сервиса <see cref="BookingService"/>
/// </summary>
public class BookingServiceTests
{
    /// <summary>
    /// Дата и время начала мероприятия
    /// </summary>
    private static readonly DateTimeOffset EventStartAt = new(new DateTime(2025, 01, 01), TimeSpan.Zero);

    /// <summary>
    /// Дата и время завершения мероприятия
    /// </summary>
    private static readonly DateTimeOffset EventEndAt = new(new DateTime(2025, 01, 02), TimeSpan.Zero);

    /// <summary>
    /// Общее количество свободных мест для мероприятия
    /// </summary>
    private const int EventTotalSeats = 3;

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
            var eventId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            var actualResult = await bookingService.Create(eventId, CancellationToken.None);

            // #### Assert ####
            actualResult.Should().NotBeNull();
            actualResult.Status.Should().Be(BookingStatus.Pending);
            @event.AvailableSeats.Should().Be(EventTotalSeats - 1);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Once);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None), Times.Once);

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
            var eventId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            var bookingResponses = new List<BookingResponse>();

            for (var i = 0; i < EventTotalSeats; i++)
            {
                var actualResult = await bookingService.Create(eventId, CancellationToken.None);

                bookingResponses.Add(actualResult);
            }

            // #### Assert ####
            bookingResponses.Should().OnlyHaveUniqueItems(x => x.Id);
            @event.AvailableSeats.Should().Be(0);

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Exactly(EventTotalSeats));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(EventTotalSeats));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None), Times.Exactly(EventTotalSeats));

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Verify(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()), Times.Exactly(EventTotalSeats));

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
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var eventId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, totalSeats, description, EventStartAt, EventEndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            var bookingResponses = new ConcurrentBag<BookingResponse>();

            var tasks = Enumerable
                .Range(0, requestCount)
                .Select(_ => Task.Run(async () =>
                {
                    var actualResult = await bookingService.Create(eventId, CancellationToken.None);
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
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Exactly(requestCount));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(totalSeats));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None), Times.Exactly(totalSeats));

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Verify(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()), Times.Exactly(totalSeats));

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
            var eventId = Guid.NewGuid();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ThrowsAsync(new NotFoundException(string.Empty));

            // #### Act ####
            var action = () => bookingService.Create(eventId, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None), Times.Once);

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

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого исключения в случае превышения количества доступных мест
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_GreaterThanAvailableSeats_ThrowConflictException()
        {
            // #### Arrange ####
            var bookingService = _autoMocker.CreateInstance<BookingService>();
            var eventId = Guid.NewGuid();

            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.GetById(It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(@event);

            _autoMocker
                .GetMock<IBookingRepository>()
                .Setup(x => x.Create(It.IsAny<Booking>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IEventRepository>()
                .Setup(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Setup(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()))
                .Returns(true);

            // #### Act ####
            for (var i = 0; i < EventTotalSeats; i++)
            {
                await bookingService.Create(eventId, CancellationToken.None);
            }

            var action = () => bookingService.Create(eventId, CancellationToken.None);

            // #### Assert ####
            await action.Should().ThrowAsync<ConflictException>();

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.GetById(eventId, CancellationToken.None), Times.Exactly(EventTotalSeats + 1));

            _autoMocker
                .GetMock<IBookingRepository>()
                .Verify(x => x.Create(It.IsAny<Booking>(), CancellationToken.None), Times.Exactly(EventTotalSeats));

            _autoMocker
                .GetMock<IEventRepository>()
                .Verify(x => x.Update(eventId, It.IsAny<Event>(), CancellationToken.None), Times.Exactly(EventTotalSeats));

            _autoMocker
                .GetMock<IPublisher<BookingCreatedMessage>>()
                .Verify(x => x.TryPublish(It.IsAny<BookingCreatedMessage>()), Times.Exactly(EventTotalSeats));

            _autoMocker.VerifyNoOtherCalls();
        }
    }
}