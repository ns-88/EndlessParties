using EndlessParties.Events.Api.App.Features.Bookings.Mappers;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.DateTime.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using EndlessParties.Shared.Utils.UserContext.Abstractions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Api.App.Features.Bookings.Create;

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
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// Провайдер <see cref="IDateTimeProvider"/>
    /// </summary>
    private readonly IDateTimeProvider _dateTimeProvider;

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
        IEventRepository eventRepository,
        IDateTimeProvider dateTimeProvider,
        IUserContextAccessor userContextAccessor,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _dateTimeProvider = dateTimeProvider;
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
        catch (Exception ex) when (ex is NotFoundException or ConflictException)
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
        var @event = await _eventRepository.GetByIdWithLock(request.EventId, cancellationToken);

        if (@event.HasStarted(_dateTimeProvider.UtcNow()))
        {
            throw new LogicException(ApplicationErrors.Bookings.EventAlreadyStarted);
        }

        var activeCount = await _bookingRepository.GetActiveCountByUserId(user.Id, cancellationToken);

        if (activeCount >= Booking.MaxActiveCount)
        {
            throw new ConflictException(string.Format(ApplicationErrors.Bookings.AvailableSeatsExceeded, Booking.MaxActiveCount));
        }

        if (!@event.TryReserveSeats())
        {
            throw new ConflictException(ApplicationErrors.Bookings.NoAvailableSeats);
        }

        var booking = new Booking(request.EventId, user.Id);

        await _bookingRepository.Create(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await _eventBus.Publish(new BookingCreatedEvent(booking.Id, user.Id), cancellationToken);

        return BookingMapper.Map(booking);
    }
}