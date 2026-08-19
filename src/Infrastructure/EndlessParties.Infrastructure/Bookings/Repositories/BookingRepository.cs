using EndlessParties.Database.Database;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Infrastructure.Bookings.Repositories;

/// <inheritdoc />
internal class BookingRepository : IBookingRepository
{
    /// <summary>
    /// Таблица <see cref="EventsDbContext.Bookings"/>
    /// </summary>
    private readonly DbSet<Booking> _bookings;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingRepository(EventsDbContext dbContext)
    {
        _bookings = dbContext.Bookings;
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
    public Task Create(Booking model, CancellationToken cancellationToken)
    {
        _bookings.Add(model);

        return Task.CompletedTask;
    }
}