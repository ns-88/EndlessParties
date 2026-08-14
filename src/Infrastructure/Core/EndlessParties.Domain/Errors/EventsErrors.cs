namespace EndlessParties.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Ошибки мероприятий (событий)
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// Наименование события не задано
        /// </summary>
        public const string NameNotSpecified = "Наименование события не задано";

        /// <summary>
        /// Общее количество мест меньше допустимого
        /// </summary>
        public const string TotalSeatsLessAllowed = "Общее количество мест меньше допустимого";

        /// <summary>
        /// Количество свободных мест имеет неверное значение
        /// </summary>
        public const string SeatsCountWrongValue = "Количество свободных мест имеет неверное значение";

        /// <summary>
        /// Количество освобождаемых мест превышает общее число доступных мест
        /// </summary>
        public const string SeatsCountGreaterThanTotalCount = "Количество освобождаемых мест превышает общее число доступных мест";

        /// <summary>
        /// Количество освобождаемых мест превышает число занятых мест
        /// </summary>
        public const string SeatsCountGreaterThanOccupiedCount = "Количество освобождаемых мест превышает число занятых мест";

        /// <summary>
        /// Длина наименования события больше допустимой
        /// </summary>
        public const string NameLongerThanAllowed = "Длина наименования события больше допустимой";

        /// <summary>
        /// Длина описания события больше допустимой
        /// </summary>
        public const string DescriptionLongerThanAllowed = "Длина описания события больше допустимой";

        /// <summary>
        /// Дата и время начала события не заданы
        /// </summary>
        public const string DateAndTimeStartNotSet = "Дата и время начала события не заданы";

        /// <summary>
        /// Дата и время завершения события не заданы
        /// </summary>
        public const string DateAndTimeCompletionNotSet = "Дата и время завершения события не заданы";

        /// <summary>
        /// Начало события не может быть позже его завершения
        /// </summary>
        public const string StartCannotLaterCompletion = "Начало события не может быть позже его завершения";

        /// <summary>
        /// Получение всех событий
        /// </summary>
        public const string ReceivingAll = "Ошибка получения всех событий";

        /// <summary>
        /// Событие не найдено
        /// </summary>
        public const string NotFound = "Событие не найдено. Id: \"{0}\"";

        /// <summary>
        /// Ошибка получения по идентификатору
        /// </summary>
        public const string ReceivingById = "Ошибка получения события по идентификатору. Id: \"{0}\"";

        /// <summary>
        /// Ошибка создания
        /// </summary>
        public const string Creation = "Ошибка создания события";

        /// <summary>
        /// Ошибка обновления
        /// </summary>
        public const string Update = "Ошибка обновления события. Id: \"{0}\"";

        /// <summary>
        /// Ошибка удаления
        /// </summary>
        public const string Deletion = "Ошибка удаления события. Id: \"{0}\"";
    }
}