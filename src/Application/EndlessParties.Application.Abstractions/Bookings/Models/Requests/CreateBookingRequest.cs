namespace EndlessParties.Application.Abstractions.Bookings.Models.Requests;

/// <summary>
/// Данные создания бронирования
/// </summary>
public class CreateBookingRequest
{
    /// <summary>
    /// Идентификатор связанного события
    /// </summary>
    public required Guid EventId { get; init; }
}