using EndlessParties.Bookings.Database;
using EndlessParties.Bookings.Domain.Errors;
using EndlessParties.Bookings.Domain.Models;
using EndlessParties.Bookings.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Bookings.Repositories;

/// <inheritdoc />
internal class BookingRepository : IBookingRepository
{
    /// <summary>
    /// Таблица <see cref="Booking"/>
    /// </summary>
    private readonly DbSet<Booking> _bookings;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingRepository(BookingsDbContext dbContext)
    {
        _bookings = dbContext.Bookings;
    }


    /// <inheritdoc />
    public Task Create(Booking model, CancellationToken cancellationToken)
    {
        _bookings.Add(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Booking> GetById(Guid id, CancellationToken cancellationToken)
    {
        Booking? booking;

        try
        {
            booking = await _bookings.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.ReceivingById, id), ex);
        }

        if (booking == null)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Bookings.NotFound, id));
        }

        return booking;
    }

    /// <inheritdoc />
    public async Task<int> GetActiveCountByUserId(Guid id, CancellationToken cancellationToken)
    {
        int bookingCount;

        try
        {
            bookingCount = await _bookings
                .Where(x => x.UserId == id)
                .CountAsync(Booking.Active, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.ReceivingActiveBookingsCount, id));
        }

        return bookingCount;
    }
}