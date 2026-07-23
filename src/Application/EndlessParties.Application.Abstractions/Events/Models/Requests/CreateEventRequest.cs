namespace EndlessParties.Application.Abstractions.Events.Models.Requests;

/// <summary>
/// Данные создания мероприятия (события)
/// </summary>
public class CreateEventRequest
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Описание
    /// </summary>
    public required string? Description { get; init; }

    /// <summary>
    /// Дата и время начала
    /// </summary>
    public required DateTimeOffset StartAt { get; init; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public required DateTimeOffset EndAt { get; init; }
}