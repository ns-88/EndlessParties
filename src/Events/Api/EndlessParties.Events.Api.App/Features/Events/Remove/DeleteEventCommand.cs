using Mediator;

namespace EndlessParties.Events.Api.App.Features.Events.Remove;

/// <summary>
/// Команда удаления мероприятия (события)
/// </summary>
public class DeleteEventCommand(Guid id) : IRequest
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}