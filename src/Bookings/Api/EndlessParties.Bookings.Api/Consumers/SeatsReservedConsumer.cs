using EndlessParties.Bookings.Api.App.Features.Reserved;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using Mediator;

namespace EndlessParties.Bookings.Api.Consumers;

/// <summary>
/// Потребитель событий резервирования места в мероприятии (событии)
/// </summary>
public class SeatsReservedConsumer : IEventBusConsumer<SeatsReservedEvent>
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="mediator"></param>
    public SeatsReservedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <inheritdoc />
    public async Task Consume(SeatsReservedEvent @event, CancellationToken cancellationToken)
    {
        var command = new SeatsReservedResultCommand
        {
            EventId = @event.EventId,
            BookingId = @event.BookingId,
            UserId = @event.UserId,
            IsSuccess = @event.IsSuccess,
            RejectReason = @event.RejectReason
        };

        await _mediator.Send(command, cancellationToken);
    }
}