using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Api.App.Features.ReserveSeats;

public partial class ReserveEventSeatsHandler
{
    /// <summary>
    /// Получен запрос резервирования свободного места в мероприятии (событии)
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен запрос резервирования свободного места в событии. Id: \"{TargetEventId}\"")]
    partial void LogReceivedToReserveEventSeat(Guid targetEventId);

    /// <summary>
    /// Свободное место в событии зарезервировано
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Свободное место в событии зарезервировано. " +
                                         "Id: \"{TargetEventId}\", мест всего: \"{TotalSeats}\", количество свободных: \"{AvailableSeats}\"")]
    partial void LogEventSeatReserved(Guid targetEventId, int totalSeats, int availableSeats);

    /// <summary>
    /// Свободное место в событии не было зарезервировано
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Свободное место в событии не было зарезервировано. Id: \"{TargetEventId}\", причина: \"{RejectReason}\"")]
    partial void LogEventSeatNotReserved(Guid targetEventId, string rejectReason);

    /// <summary>
    /// Ошибка обработки запроса резервирования свободного места в событии
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки запроса резервирования свободного места в событии. Id: \"{TargetEventId}\"")]
    partial void LogReceivedToReserveEventSeatError(Guid targetEventId, Exception exception);
}