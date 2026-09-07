namespace EndlessParties.Events.Worker.Services;

/// <summary>
/// Набор методов-расширений для класса <see cref="BookingProcessorService"/>
/// </summary>
internal static partial class BookingProcessorServiceExtensions
{
    /// <summary>
    /// Начата работа фонового сервиса обработки бронирований
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Начата работа фонового сервиса обработки бронирований")]
    public static partial void LogServiceStarted(this ILogger logger);

    /// <summary>
    /// Работа фонового сервиса обработки бронирований завершена
    /// </summary>
    [LoggerMessage(LogLevel.Information, "Работа фонового сервиса обработки бронирований завершена")]
    public static partial void LogServiceStopped(this ILogger logger);

    /// <summary>
    /// Критическая ошибка работы фонового сервиса обработки бронирований
    /// </summary>
    [LoggerMessage(LogLevel.Critical, "Критическая ошибка работы фонового сервиса обработки бронирований")]
    public static partial void LogServiceCriticalError(this ILogger logger, Exception ex);
}