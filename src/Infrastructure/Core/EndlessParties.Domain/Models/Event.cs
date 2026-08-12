using EndlessParties.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Domain.Models;

/// <summary>
/// Мероприятие (событие)
/// </summary>
public class Event
{
    /// <summary>
    /// Максимальная длина наименования
    /// </summary>
    public const int MaxTitleLength = 50;

    /// <summary>
    /// Максимальная длина описания
    /// </summary>
    public const int MaxDescriptionLength = 100;

    /// <summary>
    /// Временная блокировка методов объекта до перехода на БД
    /// </summary>
    private Lock _lock;

    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Общее количество мест
    /// </summary>
    public int TotalSeats { get; }

    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public int AvailableSeats { get; private set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public DateTimeOffset StartAt { get; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTimeOffset EndAt { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public Event(string title, int totalSeats, string? description, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        Validation(title, totalSeats, description, startAt, endAt);

        _lock = new Lock();

        Id = Guid.NewGuid();
        Title = title;
        TotalSeats = totalSeats;
        AvailableSeats = totalSeats;
        Description = description;
        StartAt = startAt;
        EndAt = endAt;
    }


    /// <summary>
    /// Резервирование свободных мест
    /// </summary>
    public bool TryReserveSeats(int count = 1)
    {
        if (count <= 0)
        {
            throw new LogicException(ApplicationErrors.Events.SeatsCountWrongValue);
        }

        using (_lock.EnterScope())
        {
            if (count > TotalSeats || count > AvailableSeats)
            {
                return false;
            }

            AvailableSeats -= count;

            return true;
        }
    }

    /// <summary>
    /// Освобождение зарезервированных мест
    /// </summary>
    public void ReleaseSeats(int count = 1)
    {
        if (count <= 0)
        {
            throw new LogicException(ApplicationErrors.Events.SeatsCountWrongValue);
        }

        if (count > TotalSeats)
        {
            throw new LogicException(ApplicationErrors.Events.SeatsCountGreaterThanTotalCount);
        }

        using (_lock.EnterScope())
        {
            if (count > TotalSeats - AvailableSeats)
            {
                throw new LogicException(ApplicationErrors.Events.SeatsCountGreaterThanOccupiedCount);
            }

            AvailableSeats += count;
        }
    }

    /// <summary>
    /// Валидация доменной сущности
    /// </summary>
    private static void Validation(string title, int totalSeats, string? description, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new LogicException(ApplicationErrors.Events.NameNotSpecified);
        }

        if (totalSeats <= 0)
        {
            throw new LogicException(ApplicationErrors.Events.TotalSeatsLessAllowed);
        }

        if (title.Length > MaxTitleLength)
        {
            throw new LogicException(ApplicationErrors.Events.NameLongerThanAllowed);
        }

        if (description is { Length: > MaxDescriptionLength })
        {
            throw new LogicException(ApplicationErrors.Events.DescriptionLongerThanAllowed);
        }

        if (startAt == default)
        {
            throw new LogicException(ApplicationErrors.Events.DateAndTimeStartNotSet);
        }

        if (endAt == default)
        {
            throw new LogicException(ApplicationErrors.Events.DateAndTimeCompletionNotSet);
        }

        if (startAt >= endAt)
        {
            throw new LogicException(ApplicationErrors.Events.StartCannotLaterCompletion);
        }
    }
}