using EndlessParties.Events.Worker.App.Features.Bookings;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using Mediator;

namespace EndlessParties.Events.Worker.Consumers;

/// <summary>
/// Обработчик событий создания новых бронирований
/// </summary>
public class BookingCreatedNewConsumer : IEventBusBatchConsumer<BookingCreatedEvent>
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingCreatedNewConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }


    /// <inheritdoc />
    public async Task Consume(IReadOnlyList<BookingCreatedEvent> events, CancellationToken cancellationToken)
    {
        await _mediator.Send(new BookingsCreatedNewCommand(events), cancellationToken);
    }
}