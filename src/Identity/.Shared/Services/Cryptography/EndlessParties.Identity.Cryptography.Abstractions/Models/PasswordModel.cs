namespace EndlessParties.Identity.Cryptography.Abstractions.Models;

/// <summary>
/// Данные пароля
/// </summary>
public readonly struct PasswordModel(string hash, string salt)
{
    /// <summary>
    /// Хэш
    /// </summary>
    public string Hash { get; } = hash;

    /// <summary>
    /// Соль
    /// </summary>
    public string Salt { get; } = salt;
}