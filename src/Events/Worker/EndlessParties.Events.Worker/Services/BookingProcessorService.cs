using EndlessParties.Events.Worker.App.Features.Bookings;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;

namespace EndlessParties.Events.Worker.Services;

/// <summary>
/// Фоновый сервис обработки бронирований
/// </summary>
internal class BookingProcessorService : BackgroundService
{
    /// <summary>
    /// <see cref="IMediator"/>
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// Логгер <see cref="ILogger"/>
    /// </summary>
    private readonly ILogger _logger;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingProcessorService(IMediator mediator, ILoggerFactory loggerFactory)
    {
        _mediator = mediator;
        _logger = loggerFactory.CreateLogger(nameof(BookingProcessorService));
    }


    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogServiceStarted();

        try
        {
            await _mediator.Send(new BookingProcessorCommand(), stoppingToken);
        }
        catch (OperationCanceledException ex) when (ex.IsCancelled(stoppingToken))
        {
            _logger.LogServiceStopped();
        }
        catch (Exception ex)
        {
            _logger.LogServiceCriticalError(ex);
        }
    }
}