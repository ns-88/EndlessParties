namespace EndlessParties.Shared.Contracts.Filters;

/// <summary>
/// Базовый класс фильтра запроса
/// </summary>
public class QueryFilterBase
{
    /// <summary>
    /// Направление сортировки
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;

    /// <summary>
    /// Номер получаемой страницы
    /// </summary>
    public int? Page { get; init; } = 1;

    /// <summary>
    /// Количество элементов на странице
    /// </summary>
    public int? PageSize { get; init; } = 10;
}