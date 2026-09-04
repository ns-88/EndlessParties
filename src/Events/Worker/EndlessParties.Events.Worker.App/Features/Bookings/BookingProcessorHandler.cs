using System.Runtime.ExceptionServices;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Messages;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Worker.App.Features.Bookings;

/// <summary>
/// Обработчик <see cref="BookingProcessorCommand"/>
/// </summary>
internal class BookingProcessorHandler : IRequestHandler<BookingProcessorCommand>
{
    /// <summary>
    /// Фабрика <see cref="IServiceScopeFactory"/>
    /// </summary>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Подписчик на сообщения <see cref="ISubscriber{T}"/>
    /// </summary>
    private readonly ISubscriber<BookingCreatedMessage> _subscriber;

    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingProcessorHandler(
        IServiceScopeFactory serviceScopeFactory,
        ISubscriber<BookingCreatedMessage> subscriber,
        ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _subscriber = subscriber;
        _logger = loggerFactory.CreateLogger(nameof(BookingProcessorHandler));
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(BookingProcessorCommand request, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var requests = _subscriber.ReadAll(cancellationToken);

        await Parallel.ForEachAsync(requests, parallelOptions, async (createdRequest, innerCancellationToken) =>
        {
            _logger.LogNewBooking(createdRequest.Id);

            await Task.Delay(TimeSpan.FromSeconds(15), innerCancellationToken);

            try
            {
                var bookingStatus = await BookingProcessing(createdRequest.Id, innerCancellationToken);

                _logger.LogBookingProcessingCompleted(createdRequest.Id, bookingStatus);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                _logger.LogBookingErrorProcessed(createdRequest.Id, ex);
            }
        });

        return Unit.Value;
    }

    /// <summary>
    /// Обработка бронирования
    /// </summary>
    private async Task<BookingStatus> BookingProcessing(Guid bookingId, CancellationToken cancellationToken)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        var bookingRepository = serviceProvider.GetRequiredService<IBookingRepository>();
        var eventRepository = serviceProvider.GetRequiredService<IEventRepository>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var strategy = unitOfWork.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(Operation, cancellationToken);

        async Task<BookingStatus> Operation(CancellationToken cancellationTokenLocal)
        {
            var booking = await bookingRepository.GetById(bookingId, cancellationTokenLocal);

            await using var transaction = await unitOfWork.BeginTransaction(cancellationTokenLocal);
            Event? @event = null;

            try
            {
                @event = await eventRepository.GetByIdWithLock(booking.EventId, cancellationTokenLocal);
            }
            catch (NotFoundException)
            {
                _logger.LogEventNotFound(booking.EventId);
                booking.Reject();
            }

            ExceptionDispatchInfo? capturedException = null;

            if (@event != null)
            {
                bool isConfirmed;

                try
                {
                    isConfirmed = await ValidateEvent(@event, cancellationTokenLocal);

                    if (!isConfirmed)
                    {
                        _logger.LogEventValidationFailed(@event.Id);
                    }
                }
                catch (Exception ex) when (!ex.IsCancelled(cancellationTokenLocal))
                {
                    isConfirmed = false;
                    capturedException = ExceptionDispatchInfo.Capture(ex);
                }

                if (isConfirmed)
                {
                    booking.Confirm();
                }
                else
                {
                    booking.Reject();
                    @event.ReleaseSeats();
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationTokenLocal);
            await transaction.CommitAsync(cancellationTokenLocal);

            capturedException?.Throw();

            return booking.Status;
        }
    }

    /// <summary>
    /// Валидация события на соответствие бизнес-правилам
    /// </summary>
    private static Task<bool> ValidateEvent(Event @event, CancellationToken cancellationToken)
    {
        var validationResult = Random.Shared.Next(100) < 20;
        var isError = Random.Shared.Next(100) < 5;

        if (isError)
        {
            throw new LogicException("Ошибка валидации события на соответствие бизнес-правилам");
        }

        return Task.FromResult(!validationResult);
    }
}