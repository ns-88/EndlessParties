using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Events.Api.Infrastructure.IntegrationTests.Repositories;

/// <summary>
/// Данные тестового случая для теста получения списка событий
/// </summary>
public class EventFilterTestCase
{
    /// <summary>
    /// Список событий для репозитория
    /// </summary>
    public required IReadOnlyList<Event> Events { get; init; }

    /// <summary>
    /// Фильтр, для которого требуется получить события 
    /// </summary>
    public required GetAllEventsFilter Filter { get; init; }

    /// <summary>
    /// Результат в виде коллекции событий
    /// </summary>
    public required CollectionResult<Event> CollectionResult { get; init; }
}