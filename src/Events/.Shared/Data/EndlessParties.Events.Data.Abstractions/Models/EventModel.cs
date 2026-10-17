namespace EndlessParties.Events.Data.Abstractions.Models;

/// <summary>
/// Модель данных мероприятия (события)
/// </summary>
public class EventModel
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Наименование
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Общее количество мест
    /// </summary>
    public required int TotalSeats { get; init; }

    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public required int AvailableSeats { get; init; }

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