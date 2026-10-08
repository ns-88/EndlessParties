namespace EndlessParties.Shared.Contracts.Bookings;

/// <summary>
/// Событие создания нового бронирования
/// </summary>
public class BookingCreatedEvent(Guid bookingId, Guid eventId, Guid userId)
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