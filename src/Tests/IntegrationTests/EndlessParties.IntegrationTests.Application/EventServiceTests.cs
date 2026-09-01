using AutoFixture;
using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Events.Services;
using EndlessParties.Database.Database;
using EndlessParties.Domain.Models;
using EndlessParties.IntegrationTests.Application.Fixtures;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.IntegrationTests.Application;

/// <summary>
/// Тесты для сервиса <see cref="EventService"/>
/// </summary>
[Trait("Category", "Integration")]
public class EventServiceTests : BaseIntegrationTest<EventsDbContext, EventFixture>
{
    /// <summary>
    /// Дата и время начала события
    /// </summary>
    private static readonly DateTimeOffset StartAt = new(new DateTime(2025, 01, 01), TimeSpan.Zero);

    /// <summary>
    /// Дата и время завершения события
    /// </summary>
    private static readonly DateTimeOffset EndAt = new(new DateTime(2025, 01, 02), TimeSpan.Zero);

    /// <summary>
    /// Общее количество мест
    /// </summary>
    private const int TotalSeats = 3;

    /// <summary>
    /// Сервис создания тестовых данных
    /// </summary>
    private readonly Fixture _fixture;


    /// <inheritdoc />
    public EventServiceTests(EventFixture fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : EventServiceTests
    {
        /// <inheritdoc />
        public Positive(EventFixture fixture) : base(fixture)
        {
        }


        /// <summary>
        /// Создание события с корректными данными и получение ожидаемого ответа
        /// </summary>
        [Fact]
        public async Task Create_CorrectData_ReturnsValidBookingResponse()
        {
            // #### Arrange ####
            var createRequest = _fixture
                .Build<CreateEventRequest>()
                .With(x => x.TotalSeats, TotalSeats)
                .With(x => x.StartAt, StartAt)
                .With(x => x.EndAt, EndAt)
                .Create();

            var expectedEvent = new Event(createRequest.Title, createRequest.TotalSeats, createRequest.Description,
                createRequest.StartAt, createRequest.EndAt);

            EventResponse actualResponse;
            var expectedResponse = new EventResponse
            {
                Id = expectedEvent.Id,
                Title = expectedEvent.Title,
                Description = expectedEvent.Description,
                AvailableSeats = expectedEvent.AvailableSeats,
                TotalSeats = expectedEvent.TotalSeats,
                StartAt = expectedEvent.StartAt,
                EndAt = expectedEvent.EndAt
            };

            // #### Act ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                actualResponse = await eventService.Create(createRequest, TestCancellationToken);
            }

            // #### Assert ####
            await using (var scope = ServiceProvider.CreateAsyncScope())
            {
                actualResponse.Should().NotBeNull();
                actualResponse.Should().BeEquivalentTo(expectedResponse, x => x.Excluding(e => e.Id));

                var context = scope.ServiceProvider.GetRequiredService<EventsDbContext>();
                var actualEvent = await context.Events.FindAsync([actualResponse.Id], TestCancellationToken);

                actualEvent.Should().NotBeNull();
                actualEvent.Should().BeEquivalentTo(expectedEvent, x => x.Excluding(e => e.Id).Excluding(e => e.RowVersion));
            }
        }
    }
}