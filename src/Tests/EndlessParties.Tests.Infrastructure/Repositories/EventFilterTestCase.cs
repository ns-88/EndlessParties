using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Tests.Infrastructure.Repositories;

/// <summary>
/// Данные тестового случая для теста получения списка событий
/// </summary>
public readonly struct EventFilterTestCase
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