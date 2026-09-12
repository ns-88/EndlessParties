using AutoFixture;
using EndlessParties.Events.Api.App.Features.Events.Create;
using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Api.App.IntegrationTests.Fixtures;
using EndlessParties.Events.Database.Database;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Shared.Utils.IntegrationTests;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EndlessParties.Events.Api.App.IntegrationTests.Features.Events.Create;

using static TestConstants;

/// <summary>
/// Тесты для обработчика <see cref="CreateEventHandler"/>
/// </summary>
[Trait("Category", "Integration")]
public class CreateEventHandlerTests : BaseIntegrationTest<EventsDbContext, EventFixture<CreateEventHandler>>
{
    /// <summary>
    /// Сервис создания тестовых данных <see cref="Fixture"/>
    /// </summary>
    private readonly Fixture _fixture;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CreateEventHandlerTests(EventFixture<CreateEventHandler> fixture) : base(fixture)
    {
        _fixture = new Fixture();
    }


    /// <summary>
    /// Позитивные тесты
    /// </summary>
    public class Positive : CreateEventHandlerTests
    {
        /// <inheritdoc />
        public Positive(EventFixture<CreateEventHandler> fixture) : base(fixture)
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
            var command = new CreateEventCommand(createRequest);

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
                var handler = scope.ServiceProvider.GetRequiredService<CreateEventHandler>();
                actualResponse = await handler.Handle(command, TestCancellationToken);
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