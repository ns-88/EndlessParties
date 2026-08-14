using System.Collections.Concurrent;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Infrastructure.Bookings.Repositories;

/// <inheritdoc />
internal class BookingRepository : IBookingRepository
{
    /// <summary>
    /// Словарь добавленных бронирований <see cref="Booking"/>
    /// </summary>
    private readonly ConcurrentDictionary<Guid, Booking> _bookings;


    /// <summary>
    /// Конструктор
    /// </summary>
    public BookingRepository()
    {
        _bookings = new ConcurrentDictionary<Guid, Booking>();
    }


    /// <inheritdoc />
    public Task<Booking> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_bookings.TryGetValue(id, out var booking))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.ObjectNotFound, id));
        }

        return Task.FromResult(booking);
    }

    /// <inheritdoc />
    public Task Create(Booking model, CancellationToken cancellationToken)
    {
        if (!_bookings.TryAdd(model.Id, model))
        {
            throw new LogicException(string.Format(ApplicationErrors.ObjectAlreadyCreated, model.Id));
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Update(Guid id, Booking model, CancellationToken cancellationToken)
    {
        if (!_bookings.ContainsKey(id))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.ObjectNotFound, id));
        }

        _bookings[id] = model;

        return Task.CompletedTask;
    }
}