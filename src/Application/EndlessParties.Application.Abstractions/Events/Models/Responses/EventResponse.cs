using EndlessParties.Application.Abstractions.Events.Models.Requests;

namespace EndlessParties.Application.Abstractions.Events.Models.Responses;

/// <summary>
/// Данные мероприятия (события)
/// </summary>
public class EventResponse : CreateEventRequest
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