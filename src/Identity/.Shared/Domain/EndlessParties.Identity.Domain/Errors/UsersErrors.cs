namespace EndlessParties.Identity.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Ошибки пользователей
    /// </summary>
    public static class UsersErrors
    {
        /// <summary>
        /// Имя пользователя (логин) не задано
        /// </summary>
        public const string NameNotSpecified = "Имя пользователя (логин) не задано";

        /// <summary>
        /// Роль пользователя имеет недопустимое значение
        /// </summary>
        public const string RoleHasWrongValue = "Роль пользователя имеет недопустимое значение";

        /// <summary>
        /// Пароль пользователя не задан
        /// </summary>
        public const string PasswordNotSpecified = "Имя пользователя (логин) не задано";
    }
}