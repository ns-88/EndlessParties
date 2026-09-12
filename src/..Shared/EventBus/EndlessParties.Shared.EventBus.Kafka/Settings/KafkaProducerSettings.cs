namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки поставщика событий Kafka
/// </summary>
public class KafkaProducerSettings
{
    /// <summary>
    /// Наименование поставщика
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Наименование топика
    /// </summary>
    public required string TopicName { get; init; }
}