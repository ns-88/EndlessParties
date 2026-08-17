using EndlessParties.Database.Database;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Contracts.Filters;
using EndlessParties.Shared.Contracts.Models;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Infrastructure.Events.Repositories;

/// <inheritdoc />
internal class EventRepository : IEventRepository
{
    /// <summary>
    /// Таблица <see cref="EventsDbContext.Events"/>
    /// </summary>
    private readonly DbSet<Event> _events;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventRepository(EventsDbContext dbContext)
    {
        _events = dbContext.Events;
    }


    /// <inheritdoc />
    public async Task<CollectionResult<Event>> GetAll(GetAllEventsFilter filter, CancellationToken cancellationToken)
    {
        CollectionResult<Event> collectionResult;

        try
        {
            // Применение фильтра запроса
            var filteredQuery = _events.ApplyFilter(filter);
            
            // Подсчет общего числа результатов запроса
            var totalCount = await filteredQuery.CountAsync(cancellationToken);

            // Применение пагинации
            var pagedQuery = filteredQuery.ApplyPagination(filter);

            // Получение финального результата
            var eventItems = await pagedQuery.AsNoTracking().ToListAsync(cancellationToken);

            collectionResult = new CollectionResult<Event>(totalCount, eventItems);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(ApplicationErrors.Events.ReceivingAll, ex);
        }

        return collectionResult;
    }

    /// <inheritdoc />
    public async Task<Event> GetById(Guid id, CancellationToken cancellationToken)
    {
        Event? @event;

        try
        {
            @event = await _events.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
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
    public async Task<Event> GetByIdWithLock(Guid id, CancellationToken cancellationToken)
    {
        Event? @event;

        try
        {
            @event = await _events
                .FromSql($"SELECT *, xmin FROM events WHERE id = {id} FOR NO KEY UPDATE")
                .SingleOrDefaultAsync(cancellationToken);
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
    public Task Create(Event model, CancellationToken cancellationToken)
    {
        _events.Add(model);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Remove(Guid id, CancellationToken cancellationToken)
    {
        int affectedRows;

        try
        {
            affectedRows = await _events
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Deletion, id), ex);
        }

        if (affectedRows == 0)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }
    }
}