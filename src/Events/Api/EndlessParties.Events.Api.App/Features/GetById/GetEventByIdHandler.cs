using EndlessParties.Events.Data.Abstractions;
using EndlessParties.Events.Data.Abstractions.Models;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.GetById;

/// <summary>
/// Обработчик <see cref="GetEventByIdQuery"/>
/// </summary>
public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, EventModel>
{
    /// <summary>
    /// Провайдер данных <see cref="IEventDataProvider"/>
    /// </summary>
    private readonly IEventDataProvider _eventDataProvider;


    /// <summary>
    /// Конструктор
    /// </summary>
    public GetEventByIdHandler(IEventDataProvider eventDataProvider)
    {
        _eventDataProvider = eventDataProvider;
    }


    /// <inheritdoc />
    public async ValueTask<EventModel> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        return await _eventDataProvider.GetById(request.Id, cancellationToken);
    }
}