using EndlessParties.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Domain.Models;

/// <summary>
/// Мероприятие (событие)
/// </summary>
public partial class Event
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
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Общее количество мест
    /// </summary>
    public int TotalSeats { get; private set; }

    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public int AvailableSeats { get; private set; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public DateTimeOffset StartAt { get; private set; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTimeOffset EndAt { get; private set; }

    /// <summary>
    /// Список связанных сущностей <see cref="Booking"/>
    /// </summary>
    public ICollection<Booking> Bookings { get; }

    /// <summary>
    /// Токен конкуренции для оптимистичной блокировки
    /// </summary>
    public uint RowVersion { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    private Event()
    {
        Title = null!;
        Bookings = new HashSet<Booking>();
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    public Event(string title, int totalSeats, string? description, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        Id = Guid.NewGuid();
        Title = ValidateTitle(title);
        TotalSeats = ValidateTotalSeats(totalSeats);
        AvailableSeats = totalSeats;
        Description = ValidateDescription(description);
        StartAt = startAt;
        EndAt = endAt;
        RowVersion = 0;
        Bookings = new HashSet<Booking>();

        ValidateStartAndEndAt(startAt, endAt);
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

        if (count > TotalSeats || count > AvailableSeats)
        {
            return false;
        }

        AvailableSeats -= count;

        return true;
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

        if (count > TotalSeats - AvailableSeats)
        {
            throw new LogicException(ApplicationErrors.Events.SeatsCountGreaterThanOccupiedCount);
        }

        AvailableSeats += count;
    }

    /// <summary>
    /// Изменение <see cref="Title"/>
    /// </summary>
    public void ChangeTitle(string title)
    {
        Title = ValidateTitle(title);
    }

    /// <summary>
    /// Изменение <see cref="TotalSeats"/>
    /// </summary>
    public void ChangeTotalSeats(int totalSeats)
    {
        TotalSeats = ValidateTotalSeats(totalSeats);
    }

    /// <summary>
    /// Изменение <see cref="Description"/>
    /// </summary>
    public void ChangeDescription(string? description)
    {
        Description = ValidateDescription(description);
    }

    /// <summary>
    /// Изменение <see cref="StartAt"/> и <see cref="EndAt"/>
    /// </summary>
    public void ChangeStartAndEndAt(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        ValidateStartAndEndAt(startAt, endAt);

        StartAt = startAt;
        EndAt = endAt;
    }
}