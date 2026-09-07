using EndlessParties.Events.Repositories.Abstractions.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Маппер для объектов <see cref="EventSearchFilter"/> и <see cref="GetAllEventsFilter"/>
/// </summary>
[Mapper]
internal static partial class EventSearchFilterMapper
{
    /// <summary>
    /// Преобразование из <see cref="EventSearchFilter"/> в <see cref="GetAllEventsFilter"/>
    /// </summary>
    [MapPropertyFromSource(nameof(GetAllEventsFilter.Offset), Use = nameof(MapPageToOffset))]
    [MapProperty(nameof(EventSearchFilter.PageSize), nameof(GetAllEventsFilter.Count))]
    public static partial GetAllEventsFilter Map(EventSearchFilter source);

    /// <summary>
    /// Преобразование номера страницы в смещение (offset)
    /// </summary>
    private static int? MapPageToOffset(EventSearchFilter source)
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