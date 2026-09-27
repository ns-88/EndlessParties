namespace EndlessParties.Events.Domain.Errors;

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
        public const string Creation = "Ошибка создания бронирования. Id события: \"{0}\", id пользователя: \"{1}\"";

        /// <summary>
        /// Нет доступных мест для бронирования
        /// </summary>
        public const string NoAvailableSeats = "Нет доступных мест для бронирования";

        /// <summary>
        /// Событие уже началось
        /// </summary>
        public const string EventAlreadyStarted = "Событие уже началось";

        /// <summary>
        /// Превышено максимальное число доступных мест для бронирования пользователем
        /// </summary>
        public const string AvailableSeatsExceeded = "Превышено максимальное число доступных мест для бронирования пользователем. " +
                                                     "Максимальное число бронирований: \"{0}\"";

        /// <summary>
        /// Получение числа активных бронирований указанного пользователя
        /// </summary>
        public const string ReceivingActiveBookingsCount = "Ошибка получения числа активных бронирований пользователя. Id пользователя: \"{0}\"";
    }
}