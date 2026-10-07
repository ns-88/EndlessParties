using EndlessParties.Bookings.Domain.Enums;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using Mediator;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Bookings.Api.App.Features.Reserved;

/// <summary>
/// Обработчик <see cref="SeatsReservedResultCommand"/>
/// </summary>
public partial class SeatsReservedResultHandler : IRequestHandler<SeatsReservedResultCommand>
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Логгер
    /// </summary>
    private readonly ILogger _logger;


    /// <summary>
    /// Конструктор
    /// </summary>
    public SeatsReservedResultHandler(
        IBookingRepository bookingRepository,
        ILoggerFactory loggerFactory,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _logger = loggerFactory.CreateLogger(nameof(SeatsReservedResultHandler));
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(SeatsReservedResultCommand request, CancellationToken cancellationToken)
    {
        var keys = new Dictionary<string, Guid>
        {
            [nameof(request.BookingId)] = request.BookingId,
            [nameof(request.EventId)] = request.EventId,
            [nameof(request.UserId)] = request.UserId
        };
        using var _ = _logger.BeginScope(keys);

        LogBookingReserveEventSeat(request.BookingId);

        try
        {
            var booking = await _bookingRepository.GetById(request.BookingId, cancellationToken);

            if (booking.Status != BookingStatus.Pending)
            {
                LogBookingInvalidStatus(request.BookingId, booking.Status);

                return Unit.Value;
            }

            if (request.IsSuccess)
            {
                booking.Confirm();
            }
            else
            {
                booking.Reject();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.IsSuccess)
            {
                LogBookingConfirm(request.BookingId);
            }
            else
            {
                var rejectReason = !string.IsNullOrWhiteSpace(request.RejectReason)
                    ? request.RejectReason
                    : "Тест ошибки не задан";
                LogBookingReject(request.BookingId, rejectReason);
            }
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            LogBookingReserveEventSeatError(request.BookingId, ex);
        }

        return Unit.Value;
    }
}