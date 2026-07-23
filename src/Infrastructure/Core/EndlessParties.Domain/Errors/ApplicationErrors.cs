namespace EndlessParties.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Объект не найден
    /// </summary>
    public const string ObjectNotFound = "Объект не найден. Id: \"{0}\"";

    /// <summary>
    /// Объект уже существует
    /// </summary>
    public const string ObjectAlreadyCreated = "Объект уже существует. Id: \"{0}\"";
}