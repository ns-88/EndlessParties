using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Events.Mappers;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Contracts.Models;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils;

namespace EndlessParties.Application.Events.Services;

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
    public async Task<EventPaginatedResponse> GetAll(GetAllEventsQueryFilter filter, CancellationToken cancellationToken)
    {
        CollectionResult<Event> collectionResult;

        try
        {
            var eventsFilter = GetAllEventsFilterMapper.Map(filter);

            collectionResult = await _eventRepository.GetAll(eventsFilter, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(ApplicationErrors.Events.ReceivingAll, ex);
        }

        return EventPaginatedResponseMapper.Map(collectionResult, filter);
    }

    /// <inheritdoc />
    public async Task<EventResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        Event @event;

        try
        {
            @event = await _eventRepository.GetById(id, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.ReceivingById, id), ex);
        }

        return EventMapper.Map(@event);
    }

    /// <inheritdoc />
    public async Task<EventResponse> Create(CreateEventRequest request, CancellationToken cancellationToken)
    {
        Event @event;

        try
        {
            var startAtUtc = request.StartAt.ToUniversalTime();
            var endAtUtc = request.EndAt.ToUniversalTime();

            @event = new Event(request.Title, request.TotalSeats, request.Description, startAtUtc, endAtUtc);

            await _eventRepository.Create(@event, cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(ApplicationErrors.Events.Creation, ex);
        }

        return EventMapper.Map(@event);
    }

    /// <inheritdoc />
    public async Task Update(Guid id, CreateEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var startAtUtc = request.StartAt.ToUniversalTime();
            var endAtUtc = request.EndAt.ToUniversalTime();

            var @event = new Event(request.Title, request.TotalSeats, request.Description, startAtUtc, endAtUtc);

            await _eventRepository.Update(id, @event, cancellationToken);
        }
        catch (NotFoundException)
        {
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Update, id), ex);
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
            throw new NotFoundException(string.Format(ApplicationErrors.Events.NotFound, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Deletion, id), ex);
        }
    }
}