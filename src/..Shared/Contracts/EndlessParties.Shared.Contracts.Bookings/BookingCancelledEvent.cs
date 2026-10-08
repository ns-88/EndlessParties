namespace EndlessParties.Shared.Contracts.Bookings;

/// <summary>
/// Событие отмены бронирования
/// </summary>
public class BookingCancelledEvent(Guid bookingId, Guid eventId, Guid userId)
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