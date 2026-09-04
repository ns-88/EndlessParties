using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Маппер для объектов <see cref="CollectionResult{T}"/> и <see cref="EventPaginatedResponse"/>
/// </summary>
internal static class EventPaginatedResponseMapper
{
    /// <summary>
    /// Преобразование из <see cref="CollectionResult{T}"/> в <see cref="EventPaginatedResponse"/>
    /// </summary>
    public static EventPaginatedResponse Map(CollectionResult<Event> source, EventSearchFilter filter)
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