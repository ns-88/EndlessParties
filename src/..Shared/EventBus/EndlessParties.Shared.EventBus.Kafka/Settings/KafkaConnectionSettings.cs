namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки подключения к шине событий на основе очереди Kafka
/// </summary>
public class KafkaConnectionSettings
{
    /// <summary>
    /// Адрес брокера
    /// </summary>
    public required string BrokerAddress { get; init; }
}