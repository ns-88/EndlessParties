namespace EndlessParties.Events.Domain.Errors
{
    /// <summary>
    /// Ошибки приложения
    /// </summary>
    public static partial class ApplicationErrors
    {
        /// <summary>
        /// Ошибки резервирования свободного места в мероприятии (событии)
        /// </summary>
        public static class Seats
        {
            /// <summary>
            /// Событие не найдено
            /// </summary>
            public const string EventNotFound = "Событие не найдено";

            /// <summary>
            /// В событии нет доступных мест
            /// </summary>
            public const string NoAvailableEventSeats = "В событии нет доступных мест";

            /// <summary>
            /// Событие уже началось
            /// </summary>
            public const string EventAlreadyStarted = "Событие уже началось";
        }
    }
}