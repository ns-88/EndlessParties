using System.Linq.Expressions;
using EndlessParties.Events.Domain.Enums;
using EndlessParties.Events.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Events.Domain.Models;

/// <summary>
/// Бронирование
/// </summary>
public class Booking
{
    /// <summary>
    /// Максимальное число бронирований у пользователя
    /// </summary>
    public const int MaxActiveCount = 10;


    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Внешний ключ для родительской сущности <see cref="Models.Event"/>
    /// </summary>
    public Guid EventId { get; }

    /// <summary>
    /// Идентификатор пользователя, создавшего бронирование
    /// </summary>
    public Guid UserId { get; }

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
    /// Признак активного бронирования
    /// </summary>
    public static Expression<Func<Booking, bool>> Active =>
        x => x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed;


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
    public Booking(Guid eventId, Guid userId)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        UserId = userId;
        Status = BookingStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
        Event = null!;
    }


    /// <summary>
    /// Подтверждение бронирования
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
    /// Отклонение бронирования
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

    /// <summary>
    /// Отмена бронирования
    /// </summary>
    public void Cancel()
    {
        if (Status != BookingStatus.Confirmed)
        {
            throw new LogicException(string.Format(ApplicationErrors.Bookings.StatusNotAcceptable, Status));
        }

        Status = BookingStatus.Canceled;
    }
}