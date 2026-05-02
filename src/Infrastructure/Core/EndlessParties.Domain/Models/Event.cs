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
    /// Идентификатор
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public DateTime StartAt { get; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTime EndAt { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public Event(string title, string? description, DateTime startAt, DateTime endAt)
    {
        Validation(title, description, startAt, endAt);

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        StartAt = startAt;
        EndAt = endAt;
    }


    /// <summary>
    /// Валидация доменной сущности
    /// </summary>
    private static void Validation(string title, string? description, DateTime startAt, DateTime endAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Наименование не задано");
        }

        if (title.Length > MaxTitleLength)
        {
            throw new InvalidOperationException("Длина наименования больше допустимой");
        }

        if (description is { Length: > MaxDescriptionLength })
        {
            throw new InvalidOperationException("Длина описания больше допустимой");
        }

        if (startAt == default)
        {
            throw new InvalidOperationException("Дата и время начала не заданы");
        }

        if (endAt == default)
        {
            throw new InvalidOperationException("Дата и время завершения не заданы");
        }

        if (startAt >= endAt)
        {
            throw new InvalidOperationException("Начало события не может быть позже его завершения");
        }
    }
}