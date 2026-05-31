using EndlessParties.Shared.Contracts.Filters;

namespace EndlessParties.Infrastructure.Abstractions.Models;

/// <summary>
/// Фильтр получения списка событий (мероприятий)
/// </summary>
public class GetAllEventsFilter : DataFilterBase
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string? Title { get; init; }

    /// <summary>
    /// Описание
    /// </summary>
    public required string? Description { get; init; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public required DateTime? From { get; init; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public required DateTime? To { get; init; }
}