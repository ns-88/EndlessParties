using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Api.App.Features.Bookings.Cancel;

/// <summary>
/// Обработчик <see cref="CancelBookingCommand"/>
/// </summary>
public class CancelBookingHandler : IRequestHandler<CancelBookingCommand>
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
    /// Сервис <see cref="IUserContextAccessor"/>
    /// </summary>
    private readonly IUserContextAccessor _userContextAccessor;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CancelBookingHandler(
        IBookingRepository bookingRepository,
        IEventRepository eventRepository,
        IUserContextAccessor userContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<Unit> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            await strategy.ExecuteAsync(ct => CancelBooking(request, ct), cancellationToken);
        }
        catch (Exception ex) when(ex is NotFoundException or ForbiddenException)
        {
            throw;
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.Cancel, request.Id), ex);
        }

        return Unit.Value;
    }

    /// <summary>
    /// Отмена бронирования
    /// </summary>
    private async Task CancelBooking(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var user = _userContextAccessor.Current;
        var booking = await _bookingRepository.GetById(request.Id, cancellationToken);

        if (booking.Status == BookingStatus.Canceled)
        {
            return;
        }

        if (booking.UserId != user.Id && user.Role != UserRole.Admin)
        {
            throw new ForbiddenException(ApplicationErrors.Bookings.NotPossibleCancelBookingFromAnotherUser);
        }

        await using var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        var @event = await _eventRepository.GetByIdWithLock(booking.EventId, cancellationToken);

        if (booking.Status == BookingStatus.Canceled)
        {
            return;
        }

        booking.Cancel();
        @event.ReleaseSeats();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}