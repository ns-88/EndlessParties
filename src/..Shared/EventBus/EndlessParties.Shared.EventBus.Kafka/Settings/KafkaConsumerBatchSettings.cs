namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки потребителя событий Kafka с поддержкой батчинга
/// </summary>
public class KafkaConsumerBatchSettings : KafkaConsumerSettings
{
    /// <summary>
    /// Размер пакета (батча)
    /// </summary>
    public required int Count { get; init; }

    /// <summary>
    /// Время ожидания накопления пакета (батча)
    /// </summary>
    public required TimeSpan Timeout { get; init; }
}