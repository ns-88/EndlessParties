using EndlessParties.Events.Api.App.Features.ReleaseSeats;
using EndlessParties.Shared.Contracts.Bookings;
using EndlessParties.Shared.EventBus.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.Consumers;

/// <summary>
/// Потребитель событий об отмене бронирования
/// </summary>
public class BookingCancelledConsumer : IEventBusBatchConsumer<BookingCancelledEvent>
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingCancelledConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <inheritdoc />
    public async Task Consume(IReadOnlyList<BookingCancelledEvent> events, CancellationToken cancellationToken)
    {
        var requests = events
            .Select(x => new ReleaseEventSeatRequest(x.BookingId, x.EventId, x.UserId))
            .ToList();
        var command = new ReleaseEventSeatsCommand(requests);

        await _mediator.Send(command, cancellationToken);
    }
}