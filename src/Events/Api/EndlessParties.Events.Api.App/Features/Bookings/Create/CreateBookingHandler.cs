using EndlessParties.Events.Api.App.Features.Bookings.Mappers;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Contracts.Events;
using EndlessParties.Shared.EventBus.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Api.App.Features.Bookings.Create;

/// <summary>
/// Обработчик <see cref="CreateBookingCommand"/>
/// </summary>
internal class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
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
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var strategy = _unitOfWork.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(Operation, cancellationToken);

        async Task<BookingResponse> Operation(CancellationToken cancellationTokenLocal)
        {
            Booking booking;

            await using var transaction = await _unitOfWork.BeginTransaction(cancellationTokenLocal);

            try
            {
                var @event = await _eventRepository.GetByIdWithLock(request.EventId, cancellationTokenLocal);

                if (!@event.TryReserveSeats())
                {
                    throw new ConflictException(ApplicationErrors.Bookings.NoAvailableSeats);
                }

                booking = new Booking(request.EventId);

                await _bookingRepository.Create(booking, cancellationTokenLocal);
                await _unitOfWork.SaveChangesAsync(cancellationTokenLocal);

                await transaction.CommitAsync(cancellationTokenLocal);

                await _eventBus.Publish(new BookingCreatedEvent(booking.Id), cancellationTokenLocal);
            }
            catch (Exception ex) when (ex is NotFoundException or ConflictException)
            {
                throw;
            }
            catch (Exception ex) when (!ex.IsCancelled(cancellationTokenLocal))
            {
                throw new LogicException(ApplicationErrors.Bookings.Creation, ex);
            }

            return BookingMapper.Map(booking);
        }
    }
}