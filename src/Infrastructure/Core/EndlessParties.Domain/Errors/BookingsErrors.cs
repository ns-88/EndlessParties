namespace EndlessParties.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Ошибки бронирований
    /// </summary>
    public static class Bookings
    {
        /// <summary>
        /// Статус не является допустимым
        /// </summary>
        public const string StatusNotAcceptable = "Статус бронирования не является допустимым. Текущий статус: \"{0}\"";

        /// <summary>
        /// Ошибка получения по идентификатору
        /// </summary>
        public const string ReceivingById = "Ошибка получения бронирования по идентификатору. Id: \"{0}\"";

        /// <summary>
        /// Бронирование не найдено
        /// </summary>
        public const string NotFound = "Бронирование не найдено. Id: \"{0}\"";

        /// <summary>
        /// Ошибка создания
        /// </summary>
        public const string Creation = "Ошибка создания бронирования";

        /// <summary>
        /// Обработка бронирования невозможна
        /// </summary>
        public const string ProcessingNotPossible = "Обработка бронирования невозможна";

        /// <summary>
        /// Нет доступных мест для бронирования
        /// </summary>
        public const string NoAvailableSeats = "Нет доступных мест для бронирования";
    }
}