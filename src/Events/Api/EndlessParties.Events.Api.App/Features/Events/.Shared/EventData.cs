namespace EndlessParties.Events.Api.App.Features.Events.Shared;

/// <summary>
/// Общие данные мероприятия (события)
/// </summary>
public class EventData
{
    /// <summary>
    /// Наименование
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Общее количество мест
    /// </summary>
    public required int TotalSeats { get; init; }

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