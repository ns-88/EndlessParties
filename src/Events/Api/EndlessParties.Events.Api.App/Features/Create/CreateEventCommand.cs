using EndlessParties.Events.Api.App.Features.Shared;
using Mediator;

namespace EndlessParties.Events.Api.App.Features.Create;

/// <summary>
/// Команда создания события (мероприятия)
/// </summary>
public class CreateEventCommand(CreateEventRequest createRequest) : IRequest<EventResponse>
{
    /// <summary>
    /// Запрос создания события (мероприятия)
    /// </summary>
    public CreateEventRequest CreateRequest { get; } = createRequest;
}