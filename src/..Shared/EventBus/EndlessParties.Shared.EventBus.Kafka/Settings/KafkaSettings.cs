using FluentValidation;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки шины событий на основе очереди Kafka
/// </summary>
public class KafkaSettings : KafkaConnectionSettings
{
    /// <summary>
    /// Поставщики
    /// </summary>
    public required IReadOnlyDictionary<Type, KafkaProducerSettings> Producers { get; init; }

    /// <summary>
    /// Потребители
    /// </summary>
    public required IReadOnlyDictionary<Type, KafkaConsumerSettings> Consumers { get; init; }


    /// <summary>
    /// Валидация
    /// </summary>
    internal void Validate() => new KafkaSettingsValidator().ValidateAndThrow(this);
}