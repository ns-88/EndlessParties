using EndlessParties.Events.Domain.Enums;

namespace EndlessParties.Events.Api.App.Features.Bookings.Create;

/// <summary>
/// Данные бронирования
/// </summary>
public class BookingResponse
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Идентификатор связанного события
    /// </summary>
    public required Guid EventId { get; init; }

    /// <summary>
    /// Статус
    /// </summary>
    public required BookingStatus Status { get; init; }
}