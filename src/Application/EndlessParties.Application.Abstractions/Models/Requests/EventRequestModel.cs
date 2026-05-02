namespace EndlessParties.Application.Abstractions.Models.Requests;

/// <summary>
/// Модель записи данных мероприятия (события)
/// </summary>
public class EventRequestModel
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
    public required DateTime StartAt { get; init; }

    /// <summary>
    /// Дата и время завершения
    /// </summary>
    public required DateTime EndAt { get; init; }
}