using Mediator;

namespace EndlessParties.Bookings.Api.App.Features.Create;

/// <summary>
/// Команда создания бронирования
/// </summary>
public class CreateBookingCommand(Guid eventId) : IRequest<BookingResponse>
{
    /// <summary>
    /// Идентификатор мероприятия (события)
    /// </summary>
    public Guid EventId { get; } = eventId;
}