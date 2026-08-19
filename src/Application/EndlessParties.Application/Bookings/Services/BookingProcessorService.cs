using System.Runtime.ExceptionServices;
using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Domain.Enums;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Application.Bookings.Services;

/// <summary>
/// Фоновый сервис обработки бронирований
/// </summary>
internal partial class BookingProcessorService : BackgroundService
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
    public BookingProcessorService(
        IServiceScopeFactory serviceScopeFactory,
        ISubscriber<BookingCreatedMessage> subscriber,
        ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _subscriber = subscriber;
        _logger = loggerFactory.CreateLogger(nameof(BookingProcessorService));
    }


    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogServiceStarted();

        try
        {
            await BookingsProcessing(stoppingToken);
        }
        catch (OperationCanceledException ex) when (ex.IsCancelled(stoppingToken))
        {
            LogServiceStopped();
        }
        catch (Exception ex)
        {
            LogServiceCriticalError(ex);
        }
    }

    /// <summary>
    /// Обработка всех поступающих бронирований
    /// </summary>
    private async Task BookingsProcessing(CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var requests = _subscriber.ReadAll(cancellationToken);

        await Parallel.ForEachAsync(requests, parallelOptions, async (request, innerCancellationToken) =>
        {
            LogNewBooking(request.Id);

            await Task.Delay(TimeSpan.FromSeconds(15), innerCancellationToken);

            try
            {
                var bookingStatus = await BookingProcessing(request.Id, innerCancellationToken);

                LogBookingProcessingCompleted(request.Id, bookingStatus);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                LogBookingErrorProcessed(request.Id, ex);
            }
        });
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
                LogEventNotFound(booking.EventId);
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
                        LogEventValidationFailed(@event.Id);
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