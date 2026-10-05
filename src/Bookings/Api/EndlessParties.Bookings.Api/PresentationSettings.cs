using EndlessParties.Shared.Utils.WebApiExtensions;
using FluentValidation;

namespace EndlessParties.Bookings.Api;

/// <summary>
/// Настройки презентационного слоя
/// </summary>
public class PresentationSettings
{
    /// <summary>
    /// Настройки аутентификации
    /// </summary>
    public required IdentitySettings Identity { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new PresentationSettingsValidator().ValidateAndThrow(this);
}