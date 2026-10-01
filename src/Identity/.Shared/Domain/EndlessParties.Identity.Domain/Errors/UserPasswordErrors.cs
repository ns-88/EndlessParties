namespace EndlessParties.Identity.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Ошибки пароля пользователя
    /// </summary>
    public static class UserPasswordErrors
    {
        /// <summary>
        /// Хэш пароля пользователя не задан
        /// </summary>
        public const string HashNotSpecified = "Хэш пароля пользователя не задан";

        /// <summary>
        /// Соль пароля пользователя не задана
        /// </summary>
        public const string SaltNotSpecified = "Соль пароля пользователя не задана";

        /// <summary>
        /// Неверные учетные данные пользователя
        /// </summary>
        public const string IncorrectCredentials = "Неверные учетные данные пользователя";
    }
}