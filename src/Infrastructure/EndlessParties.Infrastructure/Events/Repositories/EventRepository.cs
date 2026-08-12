using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Contracts.Filters;
using EndlessParties.Shared.Contracts.Models;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Infrastructure.Events.Repositories;

/// <inheritdoc />
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
internal class EventRepository : IEventRepository
{
    /// <summary>
    /// Словарь добавленных событий <see cref="Event"/>
    /// </summary>
    private readonly ConcurrentDictionary<Guid, Event> _events;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventRepository()
    {
        _events = new ConcurrentDictionary<Guid, Event>();
    }


    /// <inheritdoc />
    public Task<CollectionResult<Event>> GetAll(GetAllEventsFilter filter, CancellationToken cancellationToken)
    {
        // Применение фильтра запроса
        var filteredQuery = _events.Values.ApplyFilter(filter);

        // Подсчет общего числа результатов запроса
        var totalCount = filteredQuery.Count();

        // Применение пагинации
        var pagedQuery = filteredQuery.ApplyPagination(filter);

        // Получение финального результата
        var eventItems = pagedQuery.ToList();

        return Task.FromResult(new CollectionResult<Event>(totalCount, eventItems));
    }

    /// <inheritdoc />
    public Task<Event> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_events.TryGetValue(id, out var @event))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.ObjectNotFound, id));
        }

        return Task.FromResult(@event);
    }

    /// <inheritdoc />
    public Task<bool> Exists(Guid id, CancellationToken cancellationToken)
    {
        var exists = _events.ContainsKey(id);

        return Task.FromResult(exists);
    }

    /// <inheritdoc />
    public Task Create(Event model, CancellationToken cancellationToken)
    {
        if (!_events.TryAdd(model.Id, model))
        {
            throw new LogicException(string.Format(ApplicationErrors.ObjectAlreadyCreated, model.Id));
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Update(Guid id, Event model, CancellationToken cancellationToken)
    {
        if (!_events.ContainsKey(id))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.ObjectNotFound, id));
        }

        _events[id] = model;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Remove(Guid id, CancellationToken cancellationToken)
    {
        if (!_events.Remove(id, out _))
        {
            throw new NotFoundException(string.Format(ApplicationErrors.ObjectNotFound, id));
        }

        return Task.CompletedTask;
    }
}