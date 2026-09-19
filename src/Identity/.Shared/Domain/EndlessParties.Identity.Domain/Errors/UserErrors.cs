namespace EndlessParties.Identity.Domain.Errors;

/// <summary>
/// Ошибки приложения
/// </summary>
public static partial class ApplicationErrors
{
    /// <summary>
    /// Ошибки пользователей
    /// </summary>
    public static class UserErrors
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
        public const string PasswordNotSpecified = "Пароль пользователя не задан";

        /// <summary>
        /// Пользователь с указанными именем (логином) уже существует
        /// </summary>
        public const string SpecifiedLoginAlreadyExists = "Пользователь с указанными именем (логином) уже существует. Логин: \"{0}\"";

        /// <summary>
        /// Ошибка регистрации пользователя
        /// </summary>
        public const string RegistrationError = "Ошибка регистрации пользователя. Логин: \"{0}\"";

        /// <summary>
        /// Ошибка получения по имени (логину)
        /// </summary>
        public const string ReceivingByName = "Ошибка получения пользователя по имени (логину). Логин: \"{0}\"";

        /// <summary>
        /// Пользователь не найден
        /// </summary>
        public const string NotFound = "Пользователь не найден. Логин: \"{0}\"";

        /// <summary>
        /// Ошибка запроса аутентификации пользователя и выдачи JWT-токена
        /// </summary>
        public const string Login = "Ошибка запроса аутентификации пользователя и выдачи JWT-токена. Логин: \"{0}\"";
    }
}