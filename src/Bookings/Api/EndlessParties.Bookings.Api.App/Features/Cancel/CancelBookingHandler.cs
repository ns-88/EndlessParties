using EndlessParties.Bookings.Domain.Enums;
using EndlessParties.Bookings.Domain.Errors;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Bookings;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Bookings.Api.App.Features.Cancel;

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
    /// Сервис <see cref="IUserContextAccessor"/>
    /// </summary>
    private readonly IUserContextAccessor _userContextAccessor;

    /// <summary>
    /// Шина событий <see cref="IEventBus"/>
    /// </summary>
    private readonly IEventBus _eventBus;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public CancelBookingHandler(
        IBookingRepository bookingRepository,
        IUserContextAccessor userContextAccessor,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _userContextAccessor = userContextAccessor;
        _eventBus = eventBus;
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
        await using var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        var booking = await _bookingRepository.GetById(request.Id, cancellationToken);

        if (booking.Status == BookingStatus.Canceled)
        {
            return;
        }

        if (booking.UserId != user.Id && user.Role != UserRole.Admin)
        {
            throw new ForbiddenException(ApplicationErrors.Bookings.NotPossibleCancelBookingFromAnotherUser);
        }

        booking.Cancel();

        var cancelledEvent = new BookingCancelledEvent(request.Id, booking.EventId, user.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _eventBus.Publish(cancelledEvent, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}