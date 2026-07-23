using EndlessParties.Application.Abstractions.Bookings.Models.Messages;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Bookings.Mappers;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.MessageBus.Abstractions;

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
    /// Конструктор
    /// </summary>
    public BookingService(
        IBookingRepository bookingRepository,
        IEventRepository eventRepository,
        IPublisher<BookingCreatedMessage> publisher)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _publisher = publisher;
    }


    /// <inheritdoc />
    public async Task<BookingResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        Booking booking;

        try
        {
            booking = await _bookingRepository.GetById(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Bookings.NotFound, id));
        }
        catch (Exception ex)
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.ReceivingById, id), ex);
        }

        return BookingMapper.Map(booking);
    }

    /// <inheritdoc />
    public async Task<BookingResponse> Create(Guid eventId, CancellationToken cancellationToken)
    {
        Booking booking;

        if (!await _eventRepository.Exists(eventId, cancellationToken))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, eventId));
        }

        try
        {
            booking = new Booking(eventId);

            await _bookingRepository.Create(booking, cancellationToken);

            if (!_publisher.TryPublish(new BookingCreatedMessage(booking.Id)))
            {
                throw new LogicException(ApplicationErrors.Bookings.ProcessingNotPossible);
            }
        }
        catch (Exception ex)
        {
            throw new LogicException(ApplicationErrors.Bookings.Creation, ex);
        }

        return BookingMapper.Map(booking);
    }
}