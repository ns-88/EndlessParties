using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Shared.Contracts;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace EndlessParties.Infrastructure.Events.Repositories;

/// <summary>
/// Набор методов-расширений для класса <see cref="GetAllEventsFilter"/>
/// </summary>
internal static partial class GetAllEventsFilterExtensions
{
    [GeneratedRegex("""[^\w\s]|_""")]
    private static partial Regex ClearQueryRegex();

    /// <summary>
    /// Применение фильтра к последовательности <see cref="Event"/>
    /// </summary>
    public static IQueryable<Event> ApplyFilter(this IQueryable<Event> events, GetAllEventsFilter filter)
    {
        if (TryPrepareTextQuery(filter.Title, out var titleQuery))
        {
            events = events
                .Where(x => EF.Property<NpgsqlTsVector>(x, "title_search_vector")
                    .Matches(EF.Functions.ToTsQuery("russian", titleQuery)));
        }

        if (TryPrepareTextQuery(filter.Description, out var descriptionQuery))
        {
            events = events
                .Where(x => EF.Property<NpgsqlTsVector>(x, "description_search_vector")
                    .Matches(EF.Functions.ToTsQuery("russian", descriptionQuery)));
        }

        if (filter.From.HasValue)
        {
            events = events.Where(x => x.StartAt >= filter.From);
        }

        if (filter.To.HasValue)
        {
            events = events.Where(x => x.EndAt <= filter.To);
        }

        events = filter.SortDirection == SortDirection.Ascending
            ? events
                .OrderBy(x => x.StartAt)
                .ThenBy(x => x.Id)
            : events
                .OrderByDescending(x => x.StartAt)
                .ThenBy(x => x.Id);

        return events;
    }

    /// <summary>
    /// Подготовка текста запроса для полнотекстового поиска
    /// </summary>
    private static bool TryPrepareTextQuery(string? rawQuery, [MaybeNullWhen(false)] out string query)
    {
        query = null;

        if (string.IsNullOrWhiteSpace(rawQuery))
        {
            return false;
        }

        var cleanQuery = ClearQueryRegex().Replace(rawQuery, string.Empty).Trim();
        var words = cleanQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return false;
        }

        var prefixedWords = words.Select(x => $"{x}:*");
        query = string.Join(" & ", prefixedWords);

        return true;
    }
}