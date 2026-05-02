namespace EndlessParties.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static class ApplicationErrors
{
    /// <summary>
    /// Ошибка получения всех событий
    /// </summary>
    public const string ReceivingAllEvents = "Ошибка получения всех событий";

    /// <summary>
    /// Событие не найдено
    /// </summary>
    public const string EventNotFound = "Событие не найдено. Id: \"{0}\"";

    /// <summary>
    /// Ошибка получения события по идентификатору
    /// </summary>
    public const string ReceivingEventById = "Ошибка получения события по идентификатору. Id: \"{0}\"";

    /// <summary>
    /// Ошибка создания события
    /// </summary>
    public const string EventCreation = "Ошибка создания события";

    /// <summary>
    /// Ошибка обновления события
    /// </summary>
    public const string EventUpdate = "Ошибка обновления события. Id: \"{0}\"";

    /// <summary>
    /// Ошибка удаления события
    /// </summary>
    public const string EventRemoving = "Ошибка удаления события. Id: \"{0}\"";

    /// <summary>
    /// Событие с уже было создано
    /// </summary>
    public const string EventAlreadyCreated = "Событие с уже было создано. Id: \"{0}\"";
}