using Mediator;

namespace EndlessParties.Bookings.Api.App.Features.Reserved;

/// <summary>
/// Команда обработки результата резервирования свободных мест в событии
/// </summary>
public class SeatsReservedResultCommand : IRequest
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