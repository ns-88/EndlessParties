using System.Collections.Concurrent;
using AutoFixture;
using EndlessParties.Events.Api.App.Features.Bookings.Create;
using EndlessParties.Events.Api.App.IntegrationTests.Fixtures;
using EndlessParties.Events.Database.Database;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.Events.Api.App.IntegrationTests.Features.Bookings.Create;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="CreateBookingHandler"/>
/// </summary>
[Trait("Category", "Integration")]
public class CreateBookingHandlerTests : BaseIntegrationTest<EventsDbContext, EventFixture<CreateBookingHandler>>
{
    /// <summary>
    /// Сервис создания тестовых данных <see cref="Fixture"/>
    /// </summary>
    private readonly Fixture _fixture;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CreateBookingHandlerTests(EventFixture<CreateBookingHandler> fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive(EventFixture<CreateBookingHandler> fixture) : CreateBookingHandlerTests(fixture)
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
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();

            BookingResponse actualResponse;
            var expectedResponse = new BookingResponse
            {
                Id = Guid.NewGuid(),
                EventId = @event.Id,
                Status = BookingStatus.Pending
            };
            var expectedbooking = new Booking(@event.Id);
            var command = new CreateBookingCommand(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();
                actualResponse = await handler.Handle(command, TestCancellationToken);
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
                actualEvent.AvailableSeats.Should().Be(TotalSeats - 1);
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
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();
            var command = new CreateBookingCommand(@event.Id);

            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                context.Events.Add(@event);

                await context.SaveChangesAsync(TestCancellationToken);
            }

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();

                for (var i = 0; i < TotalSeats; i++)
                {
                    var actualResult = await handler.Handle(command, CancellationToken.None);
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
                .FromFactory((string title, string? description) => new Event(title, totalSeats, description, StartAt, EndAt))
                .Create();
            var command = new CreateBookingCommand(@event.Id);

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
                    var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();
                    var actualResult = await handler.Handle(command, TestCancellationToken);

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
    }

    /// <summary>
    /// Негативные тесты
    /// </summary>
    public class Negative(EventFixture<CreateBookingHandler> fixture) : CreateBookingHandlerTests(fixture)
    {
        /// <summary>
        /// Создание бронирования для отсутствующего события
        /// </summary>
        [Fact]
        public async Task Create_NonExistingEvent_ThrowNotFoundException()
        {
            // #### Arrange ####
            var eventId = Guid.NewGuid();
            var command = new CreateBookingCommand(eventId);

            // #### Act ####
            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();

                await handler.Handle(command, TestCancellationToken);
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
                .FromFactory((string title, string? description) => new Event(title, TotalSeats, description, StartAt, EndAt))
                .Create();
            var command = new CreateBookingCommand(@event.Id);

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
                var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();

                for (var i = 0; i < TotalSeats; i++)
                {
                    await handler.Handle(command, CancellationToken.None);
                }

                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            }

            var action = async () =>
            {
                await using var scope = ServiceProvider.CreateAsyncScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var handler = scope.ServiceProvider.GetRequiredService<CreateBookingHandler>();

                await handler.Handle(command, CancellationToken.None);
                await unitOfWork.SaveChangesAsync(TestCancellationToken);
            };

            // #### Assert ####
            await action.Should().ThrowAsync<ConflictException>();
        }
    }
}