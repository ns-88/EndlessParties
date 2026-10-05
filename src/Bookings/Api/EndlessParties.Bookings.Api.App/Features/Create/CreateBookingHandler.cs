using EndlessParties.Bookings.Api.App.Features.Mappers;
using EndlessParties.Bookings.Domain.Errors;
using EndlessParties.Bookings.Domain.Models;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Bookings;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Bookings.Api.App.Features.Create;

/// <summary>
/// Обработчик <see cref="CreateBookingCommand"/>
/// </summary>
public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
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
    public CreateBookingHandler(
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
    public async ValueTask<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        BookingResponse bookingResponse;

        try
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();

            bookingResponse = await strategy.ExecuteAsync(ct => CreateBooking(request, ct), cancellationToken);
        }
        catch (Exception ex) when (ex is ConflictException)
        {
            throw;
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.Creation, request.EventId), ex);
        }

        return bookingResponse;
    }

    /// <summary>
    /// Создание бронирования
    /// </summary>
    private async Task<BookingResponse> CreateBooking(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var user = _userContextAccessor.Current;
        await using var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        var activeCount = await _bookingRepository.GetActiveCountByUserId(user.Id, cancellationToken);

        if (activeCount >= Booking.MaxActiveCount)
        {
            throw new ConflictException(string.Format(ApplicationErrors.Bookings.AvailableSeatsExceeded, Booking.MaxActiveCount));
        }

        var booking = new Booking(request.EventId, user.Id);

        await _bookingRepository.Create(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventBus.Publish(new BookingCreatedEvent(booking.Id, request.EventId, user.Id), cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return BookingMapper.Map(booking);
    }
}