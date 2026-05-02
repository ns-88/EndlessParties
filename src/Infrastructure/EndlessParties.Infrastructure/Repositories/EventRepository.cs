using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Infrastructure.Repositories;

/// <inheritdoc />
internal class EventRepository : IEventRepository
{
    /// <summary>
    /// Словарь добавленных событий <see cref="Event"/>
    /// </summary>
    private readonly Dictionary<Guid, Event> _events;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventRepository()
    {
        _events = new Dictionary<Guid, Event>();
    }


    /// <inheritdoc />
    public Task<IReadOnlyList<Event>> GetAll(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Event>>(_events.Values.ToArray());
    }

    /// <inheritdoc />
    public Task<Event> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!_events.TryGetValue(id, out var @event))
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }

        return Task.FromResult(@event);
    }

    /// <inheritdoc />
    public Task Create(Event model, CancellationToken cancellationToken)
    {
        if (!_events.TryAdd(model.Id, model))
        {
            throw new LogicException($"Событие с уже было создано. Id: {model.Id}");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Update(Guid id, Event model, CancellationToken cancellationToken)
    {
        if (!_events.ContainsKey(id))
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }

        _events[id] = model;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Remove(Guid id, CancellationToken cancellationToken)
    {
        if (!_events.Remove(id))
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }

        return Task.CompletedTask;
    }
}