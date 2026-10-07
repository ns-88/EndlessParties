using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Api.App.Features.ReleaseSeats;

public partial class ReleaseEventSeatsHandler
{
    /// <summary>
    /// Получен запрос освобождения места в мероприятии (событии)
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен запрос освобождения места в событии. Id: \"{TargetEventId}\"")]
    partial void LogReceivedToReleaseEventSeat(Guid targetEventId);

    /// <summary>
    /// Место в событии освобождено
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Место в событии освобождено. " +
                                         "Id: \"{TargetEventId}\", мест всего: \"{TotalSeats}\", количество свободных: \"{AvailableSeats}\"")]
    partial void LogEventSeatRelease(Guid targetEventId, int totalSeats, int availableSeats);

    /// <summary>
    /// Ошибка обработки запроса освобождения места в мероприятии (событии)
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки запроса освобождения места в событии. Id: \"{TargetEventId}\"")]
    partial void LogReceivedToReleaseEventSeatError(Guid targetEventId, Exception exception);
}