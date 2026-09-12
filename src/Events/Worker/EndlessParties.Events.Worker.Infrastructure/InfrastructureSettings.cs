using EndlessParties.Shared.EventBus.Kafka.Settings;
using EndlessParties.Shared.Utils.Database.Settings;
using FluentValidation;

namespace EndlessParties.Events.Worker.Infrastructure;

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
    /// Настройки шины событий на основе очереди Kafka
    /// </summary>
    public required KafkaEventBusSettings KafkaEventBus { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new InfrastructureSettingsValidator().ValidateAndThrow(this);
}