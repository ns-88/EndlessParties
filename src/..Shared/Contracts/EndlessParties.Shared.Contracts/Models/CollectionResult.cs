namespace EndlessParties.Shared.Contracts.Models;

/// <summary>
/// Результат получения данных в виде коллекции с указанием общего числа элементов
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public class CollectionResult<T>
{
    /// <summary>
    /// Общее количество элементов
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Список полученных элементов
    /// </summary>
    public IReadOnlyList<T> Items { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public CollectionResult(int totalCount, IReadOnlyList<T> items)
    {
        TotalCount = totalCount;
        Items = items;
    }
}