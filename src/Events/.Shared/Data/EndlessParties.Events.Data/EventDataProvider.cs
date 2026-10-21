using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using EndlessParties.Events.Database;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Shared.Exceptions.Extensions;
using EndlessParties.Shared.Exceptions.Models;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Events.Data;

/// <inheritdoc />
internal class EventDataProvider : IEventDataProvider
{
    /// <summary>
    /// Таблица <see cref="Event"/>
    /// </summary>
    private readonly DbSet<Event> _events;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventDataProvider(EventsDbContext dbContext)
    {
        _events = dbContext.Set<Event>();
    }


    /// <inheritdoc />
    public async Task<EventModel> GetById(Guid id, CancellationToken cancellationToken)
    {
        EventModel? @event;

        try
        {
            @event = await _events
                .AsNoTracking()
                .ProjectToEventDto()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.ReceivingById, id), ex);
        }

        if (@event == null)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }

        return @event;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EventTopModel>> GetTop10MostFilled(CancellationToken cancellationToken)
    {
        IReadOnlyList<EventTopModel> events;

        try
        {
            events = await _events
                .AsNoTracking()
                .Where(x => x.TotalSeats > 0 && x.AvailableSeats < x.TotalSeats)
                .ProjectToEventTopDto()
                .OrderByDescending(x => x.OccupancyRate)
                .Take(10)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new LogicException(ApplicationErrors.Events.Top10MostFilledReceiving, ex);
        }

        return events;
    }
}