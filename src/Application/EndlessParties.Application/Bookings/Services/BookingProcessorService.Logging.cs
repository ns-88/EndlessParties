using EndlessParties.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Application.Bookings.Services;

internal partial class BookingProcessorService
{
    /// <summary>
    /// Полученно бронирование для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Полученно бронирование для обработки. Id брони: \"{BookingId}\"")]
    private partial void LogNewBooking(Guid bookingId);

    /// <summary>
    /// Обработка бронирования завершена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Обработка бронирования завершена. Id: \"{BookingId}\", статус: \"{BookingStatus}\"")]
    private partial void LogBookingProcessingCompleted(Guid bookingId, BookingStatus bookingStatus);

    /// <summary>
    /// Событие не найдено
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Событие не найдено. Id: \"{BookedEventId}\"")]
    private partial void LogEventNotFound(Guid bookedEventId);

    /// <summary>
    /// Валидация события на соответствие бизнес-правилам не выполнена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Валидация события на соответствие бизнес-правилам не выполнена, бронирование будет отклонено. " +
                                         "Id: \"{BookedEventId}\"")]
    private partial void LogEventValidationFailed(Guid bookedEventId);

    /// <summary>
    /// Ошибка обработки бронирования
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки бронирования. Id брони: \"{BookingId}\"")]
    private partial void LogBookingErrorProcessed(Guid bookingId, Exception ex);

    /// <summary>
    /// Начата работа фонового сервиса обработки бронирований
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Начата работа фонового сервиса обработки бронирований")]
    private partial void LogServiceStarted();

    /// <summary>
    /// Работа фонового сервиса обработки бронирований завершена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Работа фонового сервиса обработки бронирований завершена")]
    private partial void LogServiceStopped();

    /// <summary>
    /// Критическая ошибка работы фонового сервиса обработки бронирований
    /// </summary>
    [LoggerMessage(LogLevel.Critical, "Критическая ошибка работы фонового сервиса обработки бронирований")]
    private partial void LogServiceCriticalError(Exception ex);
}