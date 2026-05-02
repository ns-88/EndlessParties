using EndlessParties.Application.Abstractions.Models.Responses;
using EndlessParties.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Application.Mappers;

/// <summary>
/// Маппер для объектов <see cref="Event"/> и <see cref="EventResponseModel"/>
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class EventModelMapper
{
    /// <summary>
    /// Преобразование из <see cref="Event"/> в <see cref="EventResponseModel"/>
    /// </summary>
    public static partial EventResponseModel Map(Event source);

    /// <summary>
    /// Преобразование из списка <see cref="Event"/> в список <see cref="EventResponseModel"/>
    /// </summary>
    public static partial IReadOnlyList<EventResponseModel> MapList(IReadOnlyList<Event> sources);
}