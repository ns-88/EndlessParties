namespace EndlessParties.Shared.Contracts.Filters;

/// <summary>
/// Набор методов-расширений для класса <see cref="DataFilterBase"/>
/// </summary>
public static class DataFilterExtensions
{
    /// <summary>
    /// Применение пагинации к запросу
    /// </summary>
    public static IEnumerable<T> ApplyPagination<T>(this IEnumerable<T> query, DataFilterBase filter)
    {
        if (filter.Offset.HasValue)
        {
            query = query.Skip(filter.Offset.Value);
        }

        if (filter.Count.HasValue)
        {
            query = query.Take(filter.Count.Value);
        }

        return query;
    }
}