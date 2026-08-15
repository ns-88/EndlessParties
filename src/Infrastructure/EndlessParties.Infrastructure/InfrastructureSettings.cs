using EndlessParties.Shared.Utils.Database.Settings;
using FluentValidation;

namespace EndlessParties.Infrastructure;

/// <summary>
/// Настройки инфраструктурного слоя
/// </summary>
public class InfrastructureSettings
{
    /// <summary>
    /// Настройки подключения к базе данных "Events"
    /// </summary>
    public required DatabaseSettings EventsDatabase { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new InfrastructureSettingsValidator().ValidateAndThrow(this);
}