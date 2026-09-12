using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Events.Domain.Models;
using EndlessParties.Events.Repositories.Abstractions;
using EndlessParties.Shared.Exceptions.Models;
using EndlessParties.Shared.Utils.Database.Abstractions;
using EndlessParties.Shared.Utils.Exceptions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.Create;

/// <summary>
/// Обработчик <see cref="CreateEventCommand"/>
/// </summary>
public class CreateEventHandler : IRequestHandler<CreateEventCommand, EventResponse>
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
    public CreateEventHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }


    /// <inheritdoc />
    public async ValueTask<EventResponse> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var createRequest = request.CreateRequest;
        Event @event;

        try
        {
            var startAtUtc = createRequest.StartAt.ToUniversalTime();
            var endAtUtc = createRequest.EndAt.ToUniversalTime();

            @event = new Event(createRequest.Title, createRequest.TotalSeats, createRequest.Description, startAtUtc, endAtUtc);

            await _eventRepository.Create(@event, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (!ex.IsCancelled(cancellationToken))
        {
            throw new LogicException(ApplicationErrors.Events.Creation, ex);
        }

        return EventMapper.Map(@event);
    }
}