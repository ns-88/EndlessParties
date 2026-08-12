using System.Runtime.ExceptionServices;
using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Domain.Enums;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;
using EndlessParties.Shared.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Application.Bookings.Services;

/// <summary>
/// Фоновый сервис обработки бронирований
/// </summary>
internal partial class BookingProcessorService : BackgroundService
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;

    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// Подписчик на сообщения <see cref="ISubscriber{T}"/>
    /// </summary>
    private readonly ISubscriber<BookingCreatedMessage> _subscriber;

    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Семафор <see cref="SemaphoreSlim"/>
    /// </summary>
    private readonly SemaphoreSlim _semaphore;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingProcessorService(
        IBookingRepository bookingRepository,
        IEventRepository eventRepository,
        ISubscriber<BookingCreatedMessage> subscriber,
        ILoggerFactory loggerFactory)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _subscriber = subscriber;
        _logger = loggerFactory.CreateLogger(nameof(BookingProcessorService));
        _semaphore = new SemaphoreSlim(1, 1);
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
            await _semaphore.WaitAsync(innerCancellationToken);

            try
            {
                var bookingStatus = await BookingProcessing(request.Id, innerCancellationToken);

                LogBookingProcessingCompleted(request.Id, bookingStatus);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                LogBookingErrorProcessed(request.Id, ex);
            }
            finally
            {
                _semaphore.Release();
            }
        });
    }

    /// <summary>
    /// Обработка бронирования
    /// </summary>
    private async Task<BookingStatus> BookingProcessing(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await GetBookingById(bookingId, cancellationToken);
        Event @event;

        try
        {
            @event = await GetEventById(booking.EventId, cancellationToken);
        }
        catch (NotFoundException)
        {
            LogEventNotFound(booking.EventId);

            booking.Reject();
            await _bookingRepository.Update(bookingId, booking, cancellationToken);

            return booking.Status;
        }

        bool isConfirmed;
        ExceptionDispatchInfo? capturedException = null;

        try
        {
            isConfirmed = await ValidateEvent(@event, cancellationToken);

            if (!isConfirmed)
            {
                LogEventValidationFailed(@event.Id);
            }
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
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

            await _eventRepository.Update(booking.EventId, @event, cancellationToken);
        }

        await _bookingRepository.Update(bookingId, booking, cancellationToken);

        capturedException?.Throw();

        return booking.Status;
    }

    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    private async Task<Booking> GetBookingById(Guid id, CancellationToken cancellationToken)
    {
        Booking booking;

        try
        {
            booking = await _bookingRepository.GetById(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Bookings.NotFound, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.ReceivingById, id), ex);
        }

        return booking;
    }

    /// <summary>
    /// Получение бронирования по идентификатору
    /// </summary>
    private async Task<Event> GetEventById(Guid id, CancellationToken cancellationToken)
    {
        Event @event;

        try
        {
            @event = await _eventRepository.GetById(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.ReceivingById, id), ex);
        }

        return @event;
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

        return Task.FromResult(validationResult);
    }
}