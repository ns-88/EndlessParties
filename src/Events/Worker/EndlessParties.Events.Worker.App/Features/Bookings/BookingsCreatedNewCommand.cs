using EndlessParties.Shared.Contracts.Events;
using Mediator;

namespace EndlessParties.Events.Worker.App.Features.Bookings;

/// <summary>
/// Команда обработки поступающих бронирований
/// </summary>
public class BookingsCreatedNewCommand(IReadOnlyList<BookingCreatedEvent> createdEvents) : IRequest
{
    /// <summary>
    /// События создания новых бронирований
    /// </summary>
    public IReadOnlyList<BookingCreatedEvent> CreatedEvents { get; } = createdEvents;
}