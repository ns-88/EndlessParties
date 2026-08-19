using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Application.Events.Mappers;

/// <summary>
/// Маппер для объектов <see cref="Event"/> и <see cref="EventResponse"/>
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Both)]
internal static partial class EventMapper
{
    /// <summary>
    /// Преобразование из <see cref="Event"/> в <see cref="EventResponse"/>
    /// </summary>
    [MapperIgnoreSource(nameof(Event.Bookings))]
    [MapperIgnoreSource(nameof(Event.RowVersion))]
    public static partial EventResponse Map(Event source);

    /// <summary>
    /// Преобразование из списка <see cref="Event"/> в список <see cref="EventResponse"/>
    /// </summary>
    public static partial IReadOnlyList<EventResponse> MapList(IReadOnlyList<Event> sources);
}