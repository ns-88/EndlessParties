namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки потребителя событий Kafka
/// </summary>
public class KafkaConsumerSettings
{
    /// <summary>
    /// Тип события
    /// </summary>
    public required Type EventType { get; init; }

    /// <summary>
    /// Тип потребителя
    /// </summary>
    public required Type ConsumerType { get; init; }

    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public required string GroupId { get; init; }

    /// <summary>
    /// Наименование топика
    /// </summary>
    public required string TopicName { get; init; }
}