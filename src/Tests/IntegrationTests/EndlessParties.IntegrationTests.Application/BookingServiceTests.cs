using System.Collections.Concurrent;
using AutoFixture;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Bookings.Services;
using EndlessParties.Database.Database;
using EndlessParties.Domain.Enums;
using EndlessParties.Domain.Models;
using EndlessParties.IntegrationTests.Application.Fixtures;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.IntegrationTests.Application;

/// <summary>
/// Тесты для сервиса <see cref="BookingService"/>
/// </summary>
[Trait("Category", "Integration")]
public class BookingServiceTests : BaseIntegrationTest<EventsDbContext, EventFixture>
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
    /// Сервис создания тестовых данных <see cref="Fixture"/>
    /// </summary>
    private readonly Fixture _fixture;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingServiceTests(EventFixture fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    /// <inheritdoc />
    public class Positive(EventFixture fixture) : BookingServiceTests(fixture)
    {
        /// <summary>
        /// Создание бронирования с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            BookingResponse actualResponse;
            var expectedResponse = new BookingResponse
            {
                Id = Guid.NewGuid(),
                EventId = @event.Id,
                Status = BookingStatus.Pending
            };
            var expectedbooking = new Booking(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                actualResponse = await bookingService.Create(@event.Id, TestCancellationToken);
            }

            // #### Assert ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                actualResponse.Should().NotBeNull();
                actualResponse.Should().BeEquivalentTo(expectedResponse, x => x.Excluding(e => e.Id));

                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualEvent = await context.Events.FindAsync([@event.Id], TestCancellationToken);
                var actualBooking = await context.Bookings.FindAsync([actualResponse.Id], TestCancellationToken);

                actualBooking.Should().BeEquivalentTo(expectedbooking, x => x
                    .Excluding(e => e.Id)
                    .Excluding(e => e.Event)
                    .Excluding(e => e.CreatedAt));

                actualEvent.Should().NotBeNull();
                actualEvent.AvailableSeats.Should().Be(EventTotalSeats - 1);
            }
        }

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var bookingResponses = new List<BookingResponse>();
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                for (var i = 0; i < EventTotalSeats; i++)
                {
                    var actualResult = await bookingService.Create(@event.Id, CancellationToken.None);
                    bookingResponses.Add(actualResult);
                }
            }

            // #### Assert ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualEvent = await context.Events.FindAsync([@event.Id], TestCancellationToken);

                bookingResponses.Should().OnlyHaveUniqueItems(x => x.Id);

                actualEvent.Should().NotBeNull();
                actualEvent.AvailableSeats.Should().Be(0);
            }
        }

        /// <summary>
        /// Создание нескольких бронирований в параллельной среде для одного события и получение ожидаемого ответа
        /// </summary>
        [Theory]
        [InlineData(20, 5)]
        [InlineData(10, 10)]
        public async Task Create_ConcurrentMultipleBookings_ReturnsExpectedBookingResponse(int requestCount, int totalSeats)
        {
            // #### Arrange ####
            var bookingResponses = new ConcurrentBag<BookingResponse>();
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, totalSeats, description, EventStartAt, EventEndAt))
                .Create();

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            var tasks = Enumerable
                .Range(0, requestCount)
                .Select(_ => Task.Run(async () =>
                {
                    await using var scope = ServiceProvider.CreateAsyncScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var actualResult = await bookingService.Create(@event.Id, TestCancellationToken);

                    bookingResponses.Add(actualResult);
                }))
                .ToList();

            var results = await Task
                .WhenAll(tasks)
                .ContinueWith(x => x.Exception != null ? x.Exception.Flatten().InnerExceptions : []);

            // #### Assert ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualEvent = await context.Events.FindAsync([@event.Id], TestCancellationToken);

                results.Should().HaveCount(requestCount - totalSeats)
                    .And.AllBeOfType<ConflictException>();

                bookingResponses.Should().HaveCount(totalSeats)
                    .And.OnlyHaveUniqueItems(x => x.Id);

                actualEvent.Should().NotBeNull();
                actualEvent.AvailableSeats.Should().Be(0);
            }

        }

        /// <summary>
        /// Получение бронирования по идентификатору с возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();
            var booking = new Booking(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                await context.Events.AddAsync(@event, TestCancellationToken);
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                // #### Act ####
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var actualResult = await bookingService.GetById(booking.Id, TestCancellationToken);

                // #### Assert ####
                actualResult.Should().NotBeNull();
                actualResult.Status.Should().Be(BookingStatus.Pending);
            }
        }

        /// <summary>
        /// Получение бронирования по идентификатору с последовательным изменением статуса и возвратом ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task GetById_ChangeStatus_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();
            var booking = new Booking(@event.Id);

            BookingResponse actualResultPendingStatus;
            BookingResponse actualResultConfirmedStatus;

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                await context.Events.AddAsync(@event, TestCancellationToken);
                context.Bookings.Add(booking);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                actualResultPendingStatus = await bookingService.GetById(booking.Id, TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualBooking = await context.Bookings.FindAsync([booking.Id], TestCancellationToken);

                actualBooking!.Confirm();

                await context.SaveChangesAsync(TestCancellationToken);
            }

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                actualResultConfirmedStatus = await bookingService.GetById(booking.Id, TestCancellationToken);
            }

            // #### Assert ####
            actualResultPendingStatus.Should().NotBeNull();
            actualResultPendingStatus.Status.Should().Be(BookingStatus.Pending);

            actualResultConfirmedStatus.Should().NotBeNull();
            actualResultConfirmedStatus.Status.Should().Be(BookingStatus.Confirmed);
        }
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    /// <inheritdoc />
    public class Negative(EventFixture fixture) : BookingServiceTests(fixture)
    {
        /// <summary>
        /// Создание бронирования для отсутствующего события
        /// </summary>
        [Fact]
        public async Task Create_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var eventId = Guid.NewGuid();

            // #### Act ####
            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                await bookingService.Create(eventId, TestCancellationToken);
            };

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Получение отсутствующего бронирования по идентификатору
        /// </summary>
        [Fact]
        public async Task GetById_NonExistingBooking_ThrowNotFoundException()
        {
            // #### Arrange ####
            var bookingId = Guid.NewGuid();

            // #### Act ####
            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                await bookingService.GetById(bookingId, TestCancellationToken);
            };

            // #### Assert ####
            await action.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Создание нескольких бронирований для одного события и получение ожидаемого исключения в случае превышения количества доступных мест
        /// </summary>
        [Fact]
        public async Task Create_MultipleBookings_GreaterThanAvailableSeats_ThrowConflictException()
        {
            // #### Arrange ####
            var @event = _fixture
                .Build<Event>()
                .FromFactory((string title, string? description) => new Event(title, EventTotalSeats, description, EventStartAt, EventEndAt))
                .Create();

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                for (var i = 0; i < EventTotalSeats; i++)
                {
                    await bookingService.Create(@event.Id, CancellationToken.None);
                }

                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                await bookingService.Create(@event.Id, CancellationToken.None);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            };

            // #### Assert ####
            await action.Should().ThrowAsync<ConflictException>();
        }
    }
}