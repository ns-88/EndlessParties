using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Api.App.Features.ReserveSeats;

/// <summary>
/// Обработчик <see cref="ReserveEventSeatsCommand"/>
/// </summary>
public partial class ReserveEventSeatsHandler : IRequestHandler<ReserveEventSeatsCommand>
{
    /// <summary>
    /// Фабрика <see cref="IServiceScopeFactory"/>
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Провайдер <see cref="IDateTimeProvider"/>
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Шина событий <see cref="IEventBus"/>
    /// </summary>
    private readonly IEventBus _eventBus;

    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;


    /// <summary>
    /// Конструктор
    /// </summary>
    public ReserveEventSeatsHandler(
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IEventBus eventBus,
        ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _dateTimeProvider = dateTimeProvider;
        _eventBus = eventBus;
        _logger = loggerFactory.CreateLogger(nameof(ReserveEventSeatsHandler));
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(ReserveEventSeatsCommand request, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(request.ReserveRequests, parallelOptions, async (reserveRequest, innerCancellationToken) =>
        {
            var keys = new Dictionary<string, Guid>
            {
                [nameof(reserveRequest.BookingId)] = reserveRequest.BookingId,
                [nameof(reserveRequest.EventId)] = reserveRequest.EventId,
                [nameof(reserveRequest.UserId)] = reserveRequest.UserId
            };
            using var _ = _logger.BeginScope(keys);

            LogReceivedToReserveEventSeat(reserveRequest.EventId);

            try
            {
                await ReserveEventSeat(reserveRequest, innerCancellationToken);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                LogReceivedToReserveEventSeatError(reserveRequest.EventId, ex);
            }
        });

        return Unit.Value;
    }

    /// <summary>
    /// Резервирование свободного места в мероприятии
    /// </summary>
    private async Task ReserveEventSeat(ReserveEventSeatRequest request, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        var eventRepository = serviceProvider.GetRequiredService<IEventRepository>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var strategy = unitOfWork.CreateExecutionStrategy();

        await strategy.ExecuteAsync(Operation, cancellationToken);
        return;

        async Task Operation(CancellationToken cancellationTokenLocal)
        {
            await using var transaction = await unitOfWork.BeginTransaction(cancellationTokenLocal);
            Event? @event = null;
            var isSuccess = true;
            string? rejectReason = null;

            try
            {
                @event = await eventRepository.GetByIdWithLock(request.EventId, cancellationTokenLocal);
            }
            catch (NotFoundException)
            {
                isSuccess = false;
                rejectReason = ApplicationErrors.Seats.EventNotFound;
            }

            if (isSuccess && @event!.HasStarted(_dateTimeProvider.UtcNow()))
            {
                isSuccess = false;
                rejectReason = ApplicationErrors.Seats.EventAlreadyStarted;
            }

            if (isSuccess && !@event!.TryReserveSeats())
            {
                isSuccess = false;
                rejectReason = ApplicationErrors.Seats.NoAvailableEventSeats;
            }

            var reservedEvent = new SeatsReservedEvent
            {
                EventId = request.EventId,
                BookingId = request.BookingId,
                UserId = request.UserId,
                IsSuccess = isSuccess,
                RejectReason = rejectReason
            };

            await unitOfWork.SaveChangesAsync(cancellationTokenLocal);
            await _eventBus.Publish(reservedEvent, cancellationTokenLocal);
            await transaction.CommitAsync(cancellationTokenLocal);

            if (isSuccess)
            {
                LogEventSeatReserved(request.EventId, @event!.TotalSeats, @event.AvailableSeats);
            }
            else
            {
                LogEventSeatNotReserved(reservedEvent.EventId, rejectReason!);
            }
        }
    }
}