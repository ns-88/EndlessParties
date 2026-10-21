using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Events.Domain.Models;
using Riok.Mapperly.Abstractions;

namespace EndlessParties.Events.Data;

/// <summary>
/// Маппер для объектов <see cref="Event"/> и DTO событий
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class EventMapper
{
    /// <summary>
    /// Преобразование из <see cref="Event"/> в DTO <see cref="EventModel"/>
    /// </summary>
    public static partial IQueryable<EventModel> ProjectToEventDto(this IQueryable<Event> source);

    /// <summary>
    /// Преобразование из <see cref="Event"/> в DTO <see cref="EventTopModel"/>
    /// </summary>
    public static partial IQueryable<EventTopModel> ProjectToEventTopDto(this IQueryable<Event> source);

    /// <summary>
    /// Преобразование из <see cref="Event"/> в DTO <see cref="EventTopModel"/>
    /// </summary>
    [MapPropertyFromSource(nameof(EventTopModel.OccupancyRate), Use = nameof(MapOccupancyRate))]
    private static partial EventTopModel MapToEventTopDto(Event source);

    /// <summary>
    /// Получение значения для поля <see cref="EventTopModel.OccupancyRate"/>
    /// </summary>
    private static double MapOccupancyRate(Event source)
    {
        return (double)(source.TotalSeats - source.AvailableSeats) / source.TotalSeats;
    }
}