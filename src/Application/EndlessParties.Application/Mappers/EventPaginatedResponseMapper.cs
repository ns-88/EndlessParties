using EndlessParties.Application.Abstractions.Models.Responses;
using EndlessParties.Domain.Models;
using EndlessParties.Shared.Contracts.Models;

namespace EndlessParties.Application.Mappers;

/// <summary>
/// Маппер для объектов <see cref="CollectionResult{T}"/> и <see cref="EventPaginatedResponseModel"/>
/// </summary>
internal static class EventPaginatedResponseMapper
{
    /// <summary>
    /// Преобразование из <see cref="CollectionResult{T}"/> в <see cref="EventPaginatedResponseModel"/>
    /// </summary>
    public static EventPaginatedResponseModel Map(CollectionResult<Event> source, GetAllEventsQueryFilter filter)
    {
        var eventItems = EventMapper.MapList(source.Items);

        var totalPages = filter.PageSize > 0
            ? (int)Math.Ceiling((double)source.TotalCount / filter.PageSize.Value)
            : 0;

        return new EventPaginatedResponseModel
        {
            TotalCount = source.TotalCount,
            TotalPages = totalPages,
            PageNumber = filter.Page ?? 0,
            PageSize = filter.PageSize ?? 0,
            Items = eventItems
        };
    }
}