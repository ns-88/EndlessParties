using System.Runtime.ExceptionServices;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Worker.App.Features.Bookings;

/// <summary>
/// Обработчик <see cref="BookingsCreatedNewCommand"/>
/// </summary>
internal class BookingsCreatedNewHandler : IRequestHandler<BookingsCreatedNewCommand>
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
    public BookingsCreatedNewHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILoggerFactory loggerFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = loggerFactory.CreateLogger(nameof(BookingsCreatedNewHandler));
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(BookingsCreatedNewCommand request, CancellationToken cancellationToken)
    {
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        await Parallel.ForEachAsync(request.CreatedEvents, parallelOptions, async (createdEvent, innerCancellationToken) =>
        {
            _logger.LogNewBooking(createdEvent.Id);

            await Task.Delay(TimeSpan.FromSeconds(15), innerCancellationToken);

            try
            {
                var bookingStatus = await BookingProcessing(createdEvent.Id, innerCancellationToken);

                _logger.LogBookingProcessingCompleted(createdEvent.Id, bookingStatus);
            }
            catch (Exception ex) when (!ex.IsCancelled(innerCancellationToken))
            {
                _logger.LogBookingErrorProcessed(createdEvent.Id, ex);
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