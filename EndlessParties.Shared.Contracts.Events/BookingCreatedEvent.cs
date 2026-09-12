using System;

namespace EndlessParties.Shared.Contracts.Events;

/// <summary>
/// Событие создания нового бронирования
/// </summary>
public class BookingCreatedEvent(Guid id)
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; } = id;
}