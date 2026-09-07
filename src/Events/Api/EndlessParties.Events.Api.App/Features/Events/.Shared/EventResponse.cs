namespace EndlessParties.Events.Api.App.Features.Events.Shared;

/// <summary>
/// Данные мероприятия (события)
/// </summary>
public class EventResponse : EventData
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public required int AvailableSeats { get; init; }
}