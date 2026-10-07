namespace EndlessParties.Events.Api.App.Features.ReserveSeats;

/// <summary>
/// Запрос резервирования свободного места в мероприятии (событии)
/// </summary>
public class ReserveEventSeatRequest(Guid bookingId, Guid eventId, Guid userId)
{
    /// <summary>
    /// Идентификатор бронирования
    /// </summary>
    public Guid BookingId { get; } = bookingId;

    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; } = eventId;

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; } = userId;
}