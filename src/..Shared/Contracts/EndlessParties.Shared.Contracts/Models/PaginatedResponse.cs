namespace EndlessParties.Shared.Contracts.Models;

/// <summary>
/// Результат получения данных с поддержкой пагинации
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Количество элементов на текущей странице
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Список полученных элементов
    /// </summary>
    public required IReadOnlyList<T> Items { get; init; }
}