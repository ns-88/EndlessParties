namespace EndlessParties.Shared.Contracts.Filters;

/// <summary>
/// Базовый класс фильтра слоя данных
/// </summary>
public class DataFilterBase
{
    /// <summary>
    /// Направление сортировки
    /// </summary>
    public required SortDirection SortDirection { get; init; }

    /// <summary>
    /// Смещение в получаемых событиях
    /// </summary>
    public required int? Offset { get; init; }

    /// <summary>
    /// Количество событий, получаемых в рамках одного запроса
    /// </summary>
    public required int? Count { get; init; }
}