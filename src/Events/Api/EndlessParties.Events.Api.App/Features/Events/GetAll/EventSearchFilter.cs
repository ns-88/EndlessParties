using EndlessParties.Shared.Contracts.Filters;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Фильтр получения списка событий (мероприятий) в запросе
/// </summary>
public class EventSearchFilter : QueryFilterBase
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
    public DateTimeOffset? From { get; init; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTimeOffset? To { get; init; }
}