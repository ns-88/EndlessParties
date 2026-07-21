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
    public DateTimeOffset StartAt { get; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public DateTimeOffset EndAt { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public Event(string title, string? description, DateTimeOffset startAt, DateTimeOffset endAt)
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
    private static void Validation(string title, string? description, DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new LogicException(ApplicationErrors.Events.NameNotSpecified);
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