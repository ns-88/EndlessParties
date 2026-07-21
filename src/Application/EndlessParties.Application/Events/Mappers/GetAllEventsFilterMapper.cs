using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Infrastructure.Abstractions.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Application.Events.Mappers;

/// <summary>
/// Маппер для объектов <see cref="GetAllEventsQueryFilter"/> и <see cref="GetAllEventsFilter"/>
/// </summary>
[Mapper]
internal static partial class GetAllEventsFilterMapper
{
    /// <summary>
    /// Преобразование из <see cref="GetAllEventsQueryFilter"/> в <see cref="GetAllEventsFilter"/>
    /// </summary>
    [MapPropertyFromSource(nameof(GetAllEventsFilter.Offset), Use = nameof(MapPageToOffset))]
    [MapProperty(nameof(GetAllEventsQueryFilter.PageSize), nameof(GetAllEventsFilter.Count))]
    public static partial GetAllEventsFilter Map(GetAllEventsQueryFilter source);

    /// <summary>
    /// Преобразование номера страницы в смещение (offset)
    /// </summary>
    private static int? MapPageToOffset(GetAllEventsQueryFilter source)
    {
        return (source.Page - 1) * source.PageSize;
    }

    /// <summary>
    /// Преобразование даты и времени во время в формате UTC
    /// </summary>
    private static DateTimeOffset? MapToUtc(DateTimeOffset? source)
    {
        return source?.ToUniversalTime();
    }
}