using EndlessParties.Shared.Contracts.Filters;

namespace EndlessParties.Application.Abstractions.Models.Responses;

/// <summary>
/// Фильтр получения списка событий (мероприятий) в запросе
/// </summary>
public class GetAllEventsQueryFilter : QueryFilterBase
{
    /// <summary>
    /// Максимальное количество элементов на странице
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Наименование
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public DateTime? From { get; init; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTime? To { get; init; }
}