using EndlessParties.Shared.Utils.Ddd;

namespace EndlessParties.Events.Domain.Events;

/// <summary>
/// Доменное событие удаления мероприятия (события)
/// </summary>
public class EventDeletedDomainEvent(Guid id) : IDomainEvent
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}