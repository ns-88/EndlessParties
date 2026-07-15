using EndlessParties.Application.Abstractions.Bookings.Models.Requests;
using EndlessParties.Application.Abstractions.Bookings.Models.Responses;
using EndlessParties.Application.Abstractions.Bookings.Services;
using EndlessParties.Application.Bookings.Mappers;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Application.Bookings.Services;

/// <inheritdoc />
internal class BookingService : IBookingService
{
    /// <summary>
    /// Репозиторий <see cref="IBookingRepository"/>
    /// </summary>
    private readonly IBookingRepository _bookingRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
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
    public async Task<BookingResponse> Create(CreateBookingRequest model, CancellationToken cancellationToken)
    {
        Booking booking;

        try
        {
            booking = new Booking(model.EventId);

            await _bookingRepository.Create(booking, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new LogicException(ApplicationErrors.Bookings.Creation, ex);
        }

        return BookingMapper.Map(booking);
    }
}