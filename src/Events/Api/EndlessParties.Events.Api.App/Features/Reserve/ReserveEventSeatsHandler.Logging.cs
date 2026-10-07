using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Api.App.Features.Reserve;

public partial class ReserveEventSeatsHandler
{
    /// <summary>
    /// Получен запрос на резервирование свободного места в мероприятии (событии)
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен запрос на резервирование свободного места в событии. Id события: \"{TargetEventId}\"")]
    partial void LogReceivedToReserveEventSeat(Guid targetEventId);

    /// <summary>
    /// Свободное место в событии зарезервировано
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Свободное место в событии зарезервировано. " +
                                         "Id события: \"{TargetEventId}\", мест всего: \"{TotalSeats}\", количество свободных: \"{AvailableSeats}\"")]
    partial void LogEventSeatReserved(Guid targetEventId, int totalSeats, int availableSeats);

    /// <summary>
    /// Свободное место в событии не было зарезервировано
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Свободное место в событии не было зарезервировано. Id события: \"{TargetEventId}\", причина: \"{RejectReason}\"")]
    partial void LogEventSeatNotReserved(Guid targetEventId, string rejectReason);

    /// <summary>
    /// Ошибка обработки результата резервирования свободного места в событии
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки результата резервирования свободного места в событии. Id события: \"{TargetEventId}\"")]
    partial void LogReceivedToReserveEventSeatError(Guid targetEventId, Exception exception);
}