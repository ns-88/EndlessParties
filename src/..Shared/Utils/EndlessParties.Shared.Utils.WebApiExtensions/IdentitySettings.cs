using FluentValidation;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Настройки аутентификации
/// </summary>
public class IdentitySettings
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
    /// Потребитель
    /// </summary>
    public required string Audience { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new IdentitySettingsValidator().ValidateAndThrow(this);
}