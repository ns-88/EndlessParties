using EndlessParties.Identity.Cryptography.Abstractions.Models;

namespace EndlessParties.Identity.Cryptography.Abstractions;

/// <summary>
/// Менеджер для управления паролями
/// </summary>
public interface IPasswordManager
{
    /// <summary>
    /// Создание данных пароля
    /// </summary>
    PasswordModel CreatePassword(string password);

    /// <summary>
    /// Проверка пароля
    /// </summary>
    bool ValidatePassword(string password, PasswordModel userPassword);
}