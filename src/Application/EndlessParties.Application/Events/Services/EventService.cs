using EndlessParties.Application.Abstractions.Events.Models.Requests;
using EndlessParties.Application.Abstractions.Events.Models.Responses;
using EndlessParties.Application.Abstractions.Events.Services;
using EndlessParties.Application.Events.Mappers;
using EndlessParties.Domain.Errors;
using EndlessParties.Domain.Models;
using EndlessParties.Infrastructure.Abstractions.Repositories;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EndlessParties.Application.Events.Services;

/// <inheritdoc />
internal class EventService : IEventService
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;

    /// <summary>
    /// Единица работы <see cref="IUnitOfWork"/>
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    /// Конструктор
    /// </summary>
    public EventService(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async Task<EventPaginatedResponse> GetAll(GetAllEventsQueryFilter filter, CancellationToken cancellationToken)
    {
        var eventsFilter = GetAllEventsFilterMapper.Map(filter);
        var collectionResult = await _eventRepository.GetAll(eventsFilter, cancellationToken);

        return EventPaginatedResponseMapper.Map(collectionResult, filter);
    }

    /// <inheritdoc />
    public async Task<EventResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetById(id, cancellationToken);

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
            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
            var @event = await _eventRepository.GetById(id, cancellationToken);

            var startAtUtc = request.StartAt.ToUniversalTime();
            var endAtUtc = request.EndAt.ToUniversalTime();

            @event.ChangeTitle(request.Title);
            @event.ChangeDescription(request.Description);
            @event.ChangeTotalSeats(request.TotalSeats);
            @event.ChangeStartAndEndAt(startAtUtc, endAtUtc);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(string.Format(ApplicationErrors.Events.ModifiedByAnotherUserOrSystem, id));
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken) && ex is not NotFoundException)
        {
            throw new LogicException(string.Format(ApplicationErrors.Events.Update, id), ex);
        }
    }

    /// <inheritdoc />
    public Task Remove(Guid id, CancellationToken cancellationToken)
    {
        return _eventRepository.Remove(id, cancellationToken);
    }
}