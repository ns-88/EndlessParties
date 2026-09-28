using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.Create;

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