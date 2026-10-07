namespace EndlessParties.Shared.Contracts.Events;

/// <summary>
/// Событие резервирования места в мероприятии (событии)
/// </summary>
public class SeatsReservedEvent
{
    /// <summary>
    /// Признак успешности резервирования
    /// </summary>
    public required bool IsSuccess { get; init; }

    /// <summary>
    /// Причина ошибки, если резервирование не было произведено
    /// </summary>
    public string? RejectReason { get; init; }

    /// <summary>
    /// Идентификатор бронирования
    /// </summary>
    public required Guid BookingId { get; init; }

    /// <summary>
    /// Идентификатор события
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required Guid UserId { get; init; }
}