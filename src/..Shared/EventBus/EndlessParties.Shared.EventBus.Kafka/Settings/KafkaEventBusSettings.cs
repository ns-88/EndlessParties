using EndlessParties.Shared.EventBus.Abstractions;

namespace EndlessParties.Shared.EventBus.Kafka.Settings;

/// <summary>
/// Настройки шины событий на основе очереди Kafka
/// </summary>
public class KafkaEventBusSettings
{
    /// <summary>
    /// Адрес брокера
    /// </summary>
    public required string BrokerAddress { get; init; }

    /// <summary>
    /// Поставщики
    /// </summary>
    public IReadOnlyDictionary<Type, KafkaProducerSettings> Producers { get; }

    /// <summary>
    /// Потребители
    /// </summary>
    public IReadOnlyList<KafkaConsumerSettings> Consumers { get; }


    /// <summary>
    /// Конструктор
    /// </summary>
    public KafkaEventBusSettings()
    {
        Producers = new Dictionary<Type, KafkaProducerSettings>();
        Consumers = new List<KafkaConsumerSettings>();
    }


    /// <summary>
    /// Добавление постащика событий
    /// </summary>
    public KafkaEventBusSettings AddProducer<TEvent>(string topicName)
    {
        var producers = (Dictionary<Type, KafkaProducerSettings>)Producers;
        var type = typeof(TEvent);
        var name = $"{type.Name}-{topicName}";

        var settings = new KafkaProducerSettings
        {
            Name = name,
            TopicName = topicName
        };

        producers.Add(type, settings);

        return this;
    }

    /// <summary>
    /// Добавление потребителя событий
    /// </summary>
    public KafkaEventBusSettings AddConsumer<TEvent, TConsumer>(string groupId, string topicName)
        where TEvent : class
        where TConsumer : class, IEventBusBatchConsumer<TEvent>
    {
        var consumers = (List<KafkaConsumerSettings>)Consumers;
        var settings = new KafkaConsumerSettings
        {
            GroupId = groupId,
            TopicName = topicName,
            EventType = typeof(TEvent),
            ConsumerType = typeof(TConsumer)
        };

        consumers.Add(settings);

        return this;
    }
}