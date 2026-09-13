using EndlessParties.Events.Repositories.Abstractions;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.GetAll;

/// <summary>
/// Обработчик <see cref="GetAllEventsQuery"/>
/// </summary>
public class GetAllEventsHandler : IRequestHandler<GetAllEventsQuery, EventPaginatedResponse>
{
    /// <summary>
    /// Репозиторий <see cref="IEventRepository"/>
    /// </summary>
    private readonly IEventRepository _eventRepository;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetAllEventsHandler(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }


    /// <inheritdoc />
    public async ValueTask<EventPaginatedResponse> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
    {
        var eventsFilter = EventSearchFilterMapper.Map(request.Filter);
        var collectionResult = await _eventRepository.GetAll(eventsFilter, cancellationToken);

        return EventPaginatedResponseMapper.Map(collectionResult, request.Filter);
    }
}