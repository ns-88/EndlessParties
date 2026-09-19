using FluentValidation;

namespace EndlessParties.Identity.Cryptography.Settings;

/// <summary>
/// Настройки JWT-токена
/// </summary>
public class JwtTokenSettings
{
    /// <summary>
    /// Секретный ключ
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Издатель
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Потребители
    /// </summary>
    public required IReadOnlyList<string> Audiences { get; init; }

    /// <summary>
    /// Время жизни в минутах
    /// </summary>
    public required int Expires { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new JwtTokenSettingsValidator().ValidateAndThrow(this);
}