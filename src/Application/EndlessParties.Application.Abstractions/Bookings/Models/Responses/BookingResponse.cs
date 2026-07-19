using EndlessParties.Domain.Enums;

namespace EndlessParties.Application.Abstractions.Bookings.Models.Responses;

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