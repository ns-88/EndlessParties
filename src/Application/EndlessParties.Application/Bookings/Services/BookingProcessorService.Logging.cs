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
    /// Бронирование успешно обработано
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Бронирование успешно обработано. Id: \"{BookingId}\", статус: \"{BookingStatus}\"")]
    private partial void LogBookingSuccessfullyProcessed(Guid bookingId, BookingStatus bookingStatus);

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
    /// Ошибка получения сообщения с данными бронирования для обработки
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка получения сообщения с данными бронирования для обработки")]
    partial void LogErrorReceivingBookingCreatedMessage(Exception ex);

    /// <summary>
    /// Критическая ошибка работы фонового сервиса обработки бронирований
    /// </summary>
    [LoggerMessage(LogLevel.Critical, "Критическая ошибка работы фонового сервиса обработки бронирований")]
    private partial void LogServiceCriticalError(Exception ex);
}