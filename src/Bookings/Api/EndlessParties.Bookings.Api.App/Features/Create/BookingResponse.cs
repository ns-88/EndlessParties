using EndlessParties.Bookings.Domain.Enums;

namespace EndlessParties.Bookings.Api.App.Features.Create;

/// <summary>
/// Данные бронирования
/// </summary>
public class BookingResponse
{
    /// <summary>
    /// Идентификатор бронирования
    /// </summary>
    public required Guid BookingId { get; init; }

    /// <summary>
    /// Идентификатор связанного события
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Статус
    /// </summary>
    public required BookingStatus Status { get; init; }
}