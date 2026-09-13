using EndlessParties.Events.Api.App.Features.Events.Shared;
using EndlessParties.Events.Repositories.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.GetById;

/// <summary>
/// Обработчик <see cref="GetByIdQuery"/>
/// </summary>
public class GetByIdHandler : IRequestHandler<GetByIdQuery, EventResponse>
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetByIdHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    /// <inheritdoc />
    public async ValueTask<EventResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var @event = await _eventRepository.GetById(request.Id, cancellationToken);

        return EventMapper.Map(@event);
    }
}