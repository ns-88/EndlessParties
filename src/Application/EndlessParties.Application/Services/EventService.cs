using EndlessParties.Application.Abstractions.Models.Requests;
using EndlessParties.Application.Abstractions.Models.Responses;
using EndlessParties.Application.Abstractions.Services;
using EndlessParties.Application.Mappers;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Application.Services;

/// <inheritdoc />
internal class EventService : IEventService
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    /// <inheritdoc />
    public async Task<IReadOnlyList<EventResponseModel>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<Event> events;

        try
        {
            events = await _eventRepository.GetAll(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new LogicException("Ошибка получения всех событий", ex);
        }

        return EventModelMapper.MapList(events);
    }

    /// <inheritdoc />
    public async Task<EventResponseModel> GetById(Guid id, CancellationToken cancellationToken)
    {
        Event @event;

        try
        {
            @event = await _eventRepository.GetById(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }
        catch (Exception ex)
        {
            throw new LogicException($"Ошибка получения события по идентификатору. Id: {id}", ex);
        }

        return EventModelMapper.Map(@event);
    }

    /// <inheritdoc />
    public async Task Create(EventRequestModel model, CancellationToken cancellationToken)
    {
        try
        {
            var @event = new Event(model.Title, model.Description, model.StartAt, model.EndAt);

            await _eventRepository.Create(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new LogicException("Ошибка создания события", ex);
        }
    }

    /// <inheritdoc />
    public async Task Update(Guid id, EventRequestModel model, CancellationToken cancellationToken)
    {
        try
        {
            var @event = new Event(model.Title, model.Description, model.StartAt, model.EndAt);

            await _eventRepository.Update(id, @event, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }
        catch (Exception ex)
        {
            throw new LogicException($"Ошибка обновления события. Id: {id}", ex);
        }
    }

    /// <inheritdoc />
    public async Task Remove(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _eventRepository.Remove(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException($"Событие не найдено. Id: {id}");
        }
        catch (Exception ex)
        {
            throw new LogicException($"Ошибка обновления события. Id: {id}", ex);
        }
    }
}