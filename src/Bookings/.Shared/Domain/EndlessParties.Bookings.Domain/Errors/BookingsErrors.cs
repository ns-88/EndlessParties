namespace EndlessParties.Bookings.Domain.Errors;

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
        public const string Creation = "Ошибка создания бронирования. Id события: \"{0}\"";

        /// <summary>
        /// Ошибки отмены бронирования
        /// </summary>
        public const string Cancel = "Ошибка отмены бронирования. Id: \"{0}\"";

        /// <summary>
        /// Превышено максимальное число доступных мест для бронирования пользователем
        /// </summary>
        public const string AvailableSeatsExceeded = "Превышено максимальное количество бронирований для одного пользователя. Лимит бронирований: \"{0}\"";

        /// <summary>
        /// Получение числа активных бронирований указанного пользователя
        /// </summary>
        public const string ReceivingActiveBookingsCount = "Ошибка получения числа активных бронирований пользователя. Id пользователя: \"{0}\"";

        /// <summary>
        /// Получение бронирования невозможно, т.к. оно принадлежит другому пользователю
        /// </summary>
        public const string NotPossibleReceivingBookingFromAnotherUser = "Получение бронирования невозможно, т.к. оно принадлежит другому пользователю";

        /// <summary>
        /// Отмена бронирования невозможна, т.к. оно принадлежит другому пользователю
        /// </summary>
        public const string NotPossibleCancelBookingFromAnotherUser = "Отмена бронирования невозможна, т.к. оно принадлежит другому пользователю";
    }
}