using AutoFixture;
using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Events.Services;
using EndlessParties.Database.Database;
using EndlessParties.Events.Domain.Models;
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


    
}