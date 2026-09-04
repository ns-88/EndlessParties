using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.Update;

/// <summary>
/// Команда обновления мероприятия (события)
/// </summary>
public class UpdateEventCommand(Guid id, UpdateEventRequest updateRequest) : IRequest
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Запрос обновления события (мероприятия)
    /// </summary>
    public UpdateEventRequest UpdateRequest { get; } = updateRequest;
}