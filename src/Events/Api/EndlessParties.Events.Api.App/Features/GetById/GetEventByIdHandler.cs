using EndlessParties.Events.Api.App.Features.Shared;
using EndlessParties.Events.Repositories.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.GetById;

/// <summary>
/// Обработчик <see cref="GetEventByIdQuery"/>
/// </summary>
public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, EventResponse>
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetEventByIdHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    /// <inheritdoc />
    public async ValueTask<EventResponse> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetById(request.Id, cancellationToken);

        return EventMapper.Map(@event);
    }
}