using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Bookings.Mappers;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Application.Bookings.Services;

/// <inheritdoc />
internal class BookingService : IBookingService
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
    /// Публикатор сообщений <see cref="IPublisher{T}"/>
    /// </summary>
    private readonly IPublisher<BookingCreatedMessage> _publisher;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingService(
        IBookingRepository bookingRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        IPublisher<BookingCreatedMessage> publisher)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }


    /// <inheritdoc />
    public async Task<BookingResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetById(id, cancellationToken);

        return BookingMapper.Map(booking);
    }

    /// <inheritdoc />
    public async Task<BookingResponse> Create(Guid eventId, CancellationToken cancellationToken)
    {
        var strategy = _unitOfWork.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(Operation, cancellationToken);

        async Task<BookingResponse> Operation(CancellationToken cancellationTokenLocal)
        {
            Booking booking;

            await using var transaction = await _unitOfWork.BeginTransaction(cancellationTokenLocal);

            try
            {
                var @event = await _eventRepository.GetByIdWithLock(eventId, cancellationTokenLocal);

                if (!@event.TryReserveSeats())
                {
                    throw new ConflictException(ApplicationErrors.Bookings.NoAvailableSeats);
                }

                booking = new Booking(eventId);

                await _bookingRepository.Create(booking, cancellationTokenLocal);
                await _unitOfWork.SaveChangesAsync(cancellationTokenLocal);

                await transaction.CommitAsync(cancellationTokenLocal);

                if (!_publisher.TryPublish(new BookingCreatedMessage(booking.Id)))
                {
                    throw new LogicException(ApplicationErrors.Bookings.ProcessingNotPossible);
                }
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