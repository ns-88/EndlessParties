using EndlessParties.Domain.Enums;
using EndlessParties.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Domain.Models;

/// <summary>
/// Бронирование
/// </summary>
public class Booking
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Внешний ключ для родительской сущности <see cref="Models.Event"/>
    /// </summary>
    public Guid EventId { get; }

    /// <summary>
    /// Родительская сущность <see cref="Models.Event"/>
    /// </summary>
    public Event Event { get; }

    /// <summary>
    /// Статус
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Дата и время обработки
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }


    /// <summary>
    /// Конструктор
    /// </summary>
    private Booking()
    {
        Event = null!;
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    public Booking(Guid eventId)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        Status = BookingStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
        Event = null!;
    }


    /// <summary>
    /// Переводит бронирование в статус <see cref="BookingStatus.Confirmed"/>
    /// </summary>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.StatusNotAcceptable, Status));
        }

        Status = BookingStatus.Confirmed;
        ProcessedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Переводит бронирование в статус <see cref="BookingStatus.Rejected"/>
    /// </summary>
    public void Reject()
    {
        if (Status != BookingStatus.Pending)
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.StatusNotAcceptable, Status));
        }

        Status = BookingStatus.Rejected;
        ProcessedAt = DateTimeOffset.UtcNow;
    }
}