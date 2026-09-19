using EndlessParties.Identity.Domain.Errors;
using EndlessParties.Shared.Exceptions.Models;

namespace EndlessParties.Identity.Domain.Models;

/// <summary>
/// Пароль пользователя
/// </summary>
public record UserPassword
{
    /// <summary>
    /// Максимальная длина строки хэша
    /// </summary>
    public const int MaxHashLength = 50;

    /// <summary>
    /// Максимальная лина строки соли
    /// </summary>
    public const int MaxSaltLength = 30;


    /// <summary>
    /// Хэш
    /// </summary>
    public string Hash { get; }

    /// <summary>
    /// Соль
    /// </summary>
    public string Salt { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    private UserPassword()
    {
        Hash = string.Empty;
        Salt = string.Empty;
    }

    /// <summary>
    /// Конструктор
    /// </summary>
    public UserPassword(string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            throw new LogicException(ApplicationErrors.UserPasswordErrors.HashNotSpecified);
        }

        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new LogicException(ApplicationErrors.UserPasswordErrors.SaltNotSpecified);
        }

        Hash = hash;
        Salt = salt;
    }
}