using Mediator;

namespace EndlessParties.Events.Api.App.Features.Bookings.Create;

/// <summary>
/// Команда создания бронирования
/// </summary>
public class CreateBookingCommand(Guid eventId, Guid userId) : IRequest<BookingResponse>
{
    /// <summary>
    /// Идентификатор мероприятия (события)
    /// </summary>
    public Guid EventId { get; } = eventId;

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; } = userId;
}