using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Domain.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Application.Events.Mappers;

/// <summary>
/// Маппер для объектов <see cref="CollectionResult{T}"/> и <see cref="EventPaginatedResponse"/>
/// </summary>
internal static class EventPaginatedResponseMapper
{
    /// <summary>
    /// Преобразование из <see cref="CollectionResult{T}"/> в <see cref="EventPaginatedResponse"/>
    /// </summary>
    public static EventPaginatedResponse Map(CollectionResult<Event> source, GetAllEventsQueryFilter filter)
    {
        var eventItems = EventMapper.MapList(source.Items);

        var totalPages = filter.PageSize > 0
            ? (int)Math.Ceiling((double)source.TotalCount / filter.PageSize.Value)
            : 0;

        return new EventPaginatedResponse
        {
            TotalCount = source.TotalCount,
            TotalPages = totalPages,
            PageNumber = filter.Page ?? 0,
            PageSize = filter.PageSize ?? 0,
            Items = eventItems
        };
    }
}