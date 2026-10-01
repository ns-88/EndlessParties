namespace EndlessParties.Shared.Contracts.Events;

/// <summary>
/// Событие создания нового бронирования
/// </summary>
public class BookingCreatedEvent(Guid id, Guid userId)
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; } = userId;
}