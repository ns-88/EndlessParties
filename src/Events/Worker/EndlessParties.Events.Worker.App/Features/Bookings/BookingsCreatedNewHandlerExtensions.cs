using EndlessParties.Events.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Events.Worker.App.Features.Bookings;

/// <summary>
/// Набор методов-расширений для класса <see cref="BookingsCreatedNewHandler"/>
/// </summary>
internal static partial class BookingsCreatedNewHandlerExtensions
{
    /// <summary>
    /// Полученно бронирование для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получено бронирование для обработки. Id брони: \"{BookingId}\"")]
    public static partial void LogNewBooking(this ILogger logger, Guid bookingId);

    /// <summary>
    /// Обработка бронирования завершена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Обработка бронирования завершена. Id: \"{BookingId}\", статус: \"{BookingStatus}\"")]
    public static partial void LogBookingProcessingCompleted(this ILogger logger, Guid bookingId, BookingStatus bookingStatus);

    /// <summary>
    /// Событие не найдено
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Событие не найдено. Id: \"{BookedEventId}\"")]
    public static partial void LogEventNotFound(this ILogger logger, Guid bookedEventId);

    /// <summary>
    /// Валидация события на соответствие бизнес-правилам не выполнена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Валидация события на соответствие бизнес-правилам не выполнена, бронирование будет отклонено. " +
                                         "Id: \"{BookedEventId}\"")]
    public static partial void LogEventValidationFailed(this ILogger logger, Guid bookedEventId);

    /// <summary>
    /// Ошибка обработки бронирования
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки бронирования. Id брони: \"{BookingId}\"")]
    public static partial void LogBookingErrorProcessed(this ILogger logger, Guid bookingId, Exception ex);
}