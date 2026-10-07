using EndlessParties.Bookings.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EndlessParties.Bookings.Api.App.Features.Reserved;

public partial class SeatsReservedResultHandler
{
    /// <summary>
    /// Получен результат резервирования мест для бронирования
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Получен результат резервирования мест для бронирования. Id: \"{BookingId}\"")]
    partial void LogBookingReserveEventSeat(Guid bookingId);

    /// <summary>
    /// Бронирование подтверждено
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Бронирование подтверждено. Id: \"{BookingId}\"")]
    partial void LogBookingConfirm(Guid bookingId);

    /// <summary>
    /// Бронирование отклонено
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Бронирование отклонено. Id: \"{BookingId}\", причина: \"{RejectReason}\"")]
    partial void LogBookingReject(Guid bookingId, string rejectReason);

    /// <summary>
    /// Ошибка обработки результата резервирования мест для бронирования
    /// </summary>
    [LoggerMessage(LogLevel.Error, "Ошибка обработки результата резервирования мест для бронирования. Id: \"{BookingId}\"")]
    partial void LogBookingReserveEventSeatError(Guid bookingId, Exception exception);

    /// <summary>
    /// Недопустимый статус бронирования
    /// </summary>
    [LoggerMessage(LogLevel.Warning, "Недопустимый статус бронирования, обработка отклонена. Id: \"{BookingId}\", статус: \"{BookingStatus}\"")]
    partial void LogBookingInvalidStatus(Guid bookingId, BookingStatus bookingStatus);
}