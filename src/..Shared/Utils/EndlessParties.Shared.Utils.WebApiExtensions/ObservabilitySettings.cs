using FluentValidation;

namespace EndlessParties.Shared.Utils.WebApiExtensions;

/// <summary>
/// Настройки наблюдаемости
/// </summary>
internal class ObservabilitySettings
{
    /// <summary>
    /// Адрес отправления данных для OpenTelemetry
    /// </summary>
    public required string OltpEndpoint { get; init; }

    /// <summary>
    /// Наименование сервиса
    /// </summary>
    public required string ServiceName { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new ObservabilitySettingsValidator().ValidateAndThrow(this);
}