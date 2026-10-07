using EndlessParties.Events.Api.App.Features.Reserve;
using EndlessParties.Shared.Contracts.Bookings;
using EndlessParties.Shared.EventBus.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.Consumers;

/// <summary>
/// Потребитель событий о создании бронирования
/// </summary>
public class BookingCreatedConsumer : IEventBusBatchConsumer<BookingCreatedEvent>
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <inheritdoc />
    public async Task Consume(IReadOnlyList<BookingCreatedEvent> events, CancellationToken cancellationToken)
    {
        var requests = events
            .Select(x => new ReserveEventSeatRequest(x.BookingId, x.EventId, x.UserId))
            .ToList();
        var command = new ReserveEventSeatsCommand(requests);

        await _mediator.Send(command, cancellationToken);
    }
}