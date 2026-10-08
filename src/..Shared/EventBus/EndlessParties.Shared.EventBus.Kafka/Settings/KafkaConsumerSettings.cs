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
    /// Тип потребителя-посредника
    /// </summary>
    public required Type ProxyType { get; init; }

    /// <summary>
    /// Тип целевого потребителя (абстракция)
    /// </summary>
    public required Type TargetInterfaceType { get; init; }

    /// <summary>
    /// Тип целевого потребителя (реализация)
    /// </summary>
    public required Type TargetImplementationType { get; init; }

    /// <summary>
    /// Идентификатор группы
    /// </summary>
    public required string GroupId { get; init; }

    /// <summary>
    /// Наименование топика
    /// </summary>
    public required string TopicName { get; init; }
}