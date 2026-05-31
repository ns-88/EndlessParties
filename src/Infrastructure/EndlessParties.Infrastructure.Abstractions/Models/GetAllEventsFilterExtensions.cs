using EndlessParties.Domain.Models;
using EndlessParties.Shared.Contracts;

namespace EndlessParties.Infrastructure.Abstractions.Models;

/// <summary>
/// Набор методов-расширений для класса <see cref="GetAllEventsFilter"/>
/// </summary>
public static class GetAllEventsFilterExtensions
{
    /// <summary>
    /// Применение фильтра к последовательности <see cref="Event"/>
    /// </summary>
    public static IEnumerable<Event> ApplyFilter(this IEnumerable<Event> events, GetAllEventsFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            events = events.Where(x => x.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Description))
        {
            events = events.Where(x => x.Description != null &&
                                       x.Description.Contains(filter.Description, StringComparison.OrdinalIgnoreCase));
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
}