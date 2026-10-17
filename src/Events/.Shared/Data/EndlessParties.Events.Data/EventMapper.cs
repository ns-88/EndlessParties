using System.Linq;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Events.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Events.Data;

/// <summary>
/// Маппер для объектов <see cref="Event"/> и <see cref="EventModel"/>
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class EventMapper
{
    /// <summary>
    /// Преобразование из <see cref="Event"/> в DTO <see cref="EventModel"/>
    /// </summary>
    public static partial IQueryable<EventModel> ProjectToDto(this IQueryable<Event> source);
}