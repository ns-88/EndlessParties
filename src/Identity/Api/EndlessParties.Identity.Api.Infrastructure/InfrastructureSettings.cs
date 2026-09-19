using EndlessParties.Shared.Utils.Database.Settings;
using FluentValidation;

namespace EndlessParties.Identity.Api.Infrastructure;

/// <summary>
/// Настройки инфраструктурного слоя
/// </summary>
public class InfrastructureSettings
{
    /// <summary>
    /// Настройки подключения к базе данных "Identity"
    /// </summary>
    public required DatabaseSettings IdentityDatabase { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new InfrastructureSettingsValidator().ValidateAndThrow(this);
}