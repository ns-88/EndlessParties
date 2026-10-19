namespace EndlessParties.Events.Data.Abstractions.Models;

/// <summary>
/// Модель данных мероприятия (события) с указанием коэффициента заполненности мест
/// </summary>
public class EventTopModel : EventModel
{
    /// <summary>
    /// Коэффициент заполненности мест
    /// </summary>
    public required double OccupancyRate { get; init; }
}