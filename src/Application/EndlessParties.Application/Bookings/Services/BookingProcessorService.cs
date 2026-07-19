using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.MessageBus.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Application.Bookings.Services;

internal partial class BookingProcessorService : BackgroundService
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;

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
        IBookingRepository bookingRepository,
        ISubscriber<BookingCreatedMessage> subscriber,
        ILoggerFactory loggerFactory)
    {
        _bookingRepository = bookingRepository;
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
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
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
        while (!cancellationToken.IsCancellationRequested)
        {
            BookingCreatedMessage message;

            try
            {
                message = await _subscriber.Read(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogErrorReceivingBookingCreatedMessage(ex);

                continue;
            }

            LogNewBooking(message.Id);

            try
            {
                var booking = await _bookingRepository.GetById(message.Id, cancellationToken);

                await BookingProcessing(booking, cancellationToken);

                LogBookingSuccessfullyProcessed(message.Id, booking.Status);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                LogBookingErrorProcessed(message.Id, ex);
            }
        }
    }

    /// <summary>
    /// Обработка бронирования
    /// </summary>
    private async Task BookingProcessing(Booking booking, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        booking.Confirm();

        await _bookingRepository.Update(booking.Id, booking, cancellationToken);
    }
}