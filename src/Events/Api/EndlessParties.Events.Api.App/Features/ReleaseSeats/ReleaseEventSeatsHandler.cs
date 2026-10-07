using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Api.App.Features.ReleaseSeats;

/// <summary>
/// Обработчик <see cref="ReleaseEventSeatsCommand"/>
/// </summary>
public partial class ReleaseEventSeatsHandler : IRequestHandler<ReleaseEventSeatsCommand>
{
    /// <summary>
    /// Фабрика <see cref="IServiceScopeFactory"/>
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;


    /// <summary>
    /// Конструктор
    /// </summary>
    public ReleaseEventSeatsHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(ReleaseEventSeatsHandler));
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(ReleaseEventSeatsCommand request, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(request.ReleaseRequests, parallelOptions, async (releaseRequest, innerCancellationToken) =>
        {
            var keys = new Dictionary<string, Guid>
            {
                [nameof(releaseRequest.BookingId)] = releaseRequest.BookingId,
                [nameof(releaseRequest.EventId)] = releaseRequest.EventId,
                [nameof(releaseRequest.UserId)] = releaseRequest.UserId
            };
            using var _ = _logger.BeginScope(keys);

            LogReceivedToReleaseEventSeat(releaseRequest.EventId);

            try
            {
                await ReleaseEventSeat(releaseRequest, innerCancellationToken);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                LogReceivedToReleaseEventSeatError(releaseRequest.EventId, ex);
            }
        });

        return Unit.Value;
    }

    /// <summary>
    /// Освобождение места в мероприятии
    /// </summary>
    private async Task ReleaseEventSeat(ReleaseEventSeatRequest request, CancellationToken cancellationToken)
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
            var @event = await eventRepository.GetByIdWithLock(request.EventId, cancellationTokenLocal);

            @event.ReleaseSeats();

            await unitOfWork.SaveChangesAsync(cancellationTokenLocal);
            await transaction.CommitAsync(cancellationTokenLocal);

            LogEventSeatRelease(@event.Id, @event.TotalSeats, @event.AvailableSeats);
        }
    }
}