using EndlessParties.Shared.Utils.Ddd;

namespace EndlessParties.Events.Domain.Events;

/// <summary>
/// Доменное событие изменения мероприятия (события)
/// </summary>
public class EventUpdatedDomainEvent(Guid id) : IDomainEvent
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}